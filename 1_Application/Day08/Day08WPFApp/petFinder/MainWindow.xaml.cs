using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using petFinder.Logics;
using petFinder.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using System.Web.UI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

// db삽입 업데이트 다시하기
namespace petFinder
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // 로드
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SelectDB();
            using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
            {
                conn.Open();
                var query = @"SELECT DISTINCT ty3Process
                                FROM animalinfo
                                ORDER BY 1";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                List<string> processList = new List<string>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    processList.Add(Convert.ToString(row[0]));
                }

                CboPrcs.ItemsSource = processList;
            }
        }

        //  실시간 조회 버튼
        private async void BtnReq_Click(object sender, RoutedEventArgs e)
        {
            string openApiUrl = "http://apis.data.go.kr/6260000/BusanPetAnimalInfoService/getPetAnimalInfo"; // URL
            openApiUrl += "?ServiceKey=" + "Hp7RL4tCw0cXBMTYsWCTrydbix%2Fqtqe4%2Bu5yRNze4LKbniVQhVKmNWMk8IxYObz6%2FEB41Vo47zCdEVUVRfAvsA%3D%3D"; // Service Key
            openApiUrl += "&pageNo=1";
            openApiUrl += "&numOfRows=50";
            openApiUrl += "&resultType=json";

            string result = string.Empty;

            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            #region < API 요청, 응답 >
            try
            {
                req = WebRequest.Create(openApiUrl);
                res = await req.GetResponseAsync();
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd();

                // Debug.Write(result);
            }
            catch (Exception ex)
            {

                await Commons.ShowMessageAsync("오류", $"OpenAPI조회 오류{ex.Message}");
            }
            #endregion

            #region < JSON 처리 >
            var jsonResult = JObject.Parse(result);
            // Debug.WriteLine(jsonResult);

            var resultCode = Convert.ToInt32(jsonResult["getPetAnimalInfo"]["header"]["resultCode"]);

            var animalInfoDetail = new List<AminamlInfoDetail>();
            
            try
            {
                if( resultCode == 0 ) // API 결과 정상이면 처리
                {
                    var data = jsonResult["getPetAnimalInfo"]["body"]["items"]["item"];
                    var json_arry = data as JArray;

                    
                    foreach (var item in json_arry )
                    {
                        animalInfoDetail.Add(new AminamlInfoDetail
                        {
                            Sj = Convert.ToString(item["sj"]),
                            WritngDe = Convert.ToDateTime(item["writngDe"]),
                            Cn = Convert.ToString(item["cn"]),
                            Ty3Date = Convert.ToString(item["ty3Date"]),
                            Ty3Place = Convert.ToString(item["ty3Place"]),
                            Ty3Kind = Convert.ToString(item["ty3Kind"]),
                            Ty3Sex = Convert.ToString(item["ty3Sex"]),
                            Ty3Process = Convert.ToString(item["ty3Process"]),
                            Ty3Ingye = Convert.ToString(item["ty3Ingye"]),
                            Ty3Insu = Convert.ToString(item["ty3Insu"]),
                            Ty3Picture = Convert.ToString(item["ty3Picture"])
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"JSON 처리 오류 {ex.Message}");

            }

            #endregion

            #region < DB 연결 >
            try
            {
                using(MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if(conn.State == ConnectionState.Closed) conn.Open();


                    var query = @"INSERT IGNORE INTO animalinfo
                                         (Sj,
                                         WritngDe,
                                         Cn,
                                         Ty3Date,
                                         Ty3Place,
                                         Ty3Kind,
                                         Ty3Sex,
                                         Ty3Process,
                                         Ty3Ingye,
                                         Ty3Insu,
                                         Ty3Picture)
                                 VALUES
                                         (@Sj,
                                         @WritngDe,
                                         @Cn,
                                         @Ty3Date,
                                         @Ty3Place,
                                         @Ty3Kind,
                                         @Ty3Sex,
                                         @Ty3Process,
                                         @Ty3Ingye,
                                         @Ty3Insu,
                                         @Ty3Picture)";
                                

                    var insRes = 0; // 신규 실종 반려동물 건수
                    animalInfoDetail.Reverse();
                    foreach(AminamlInfoDetail item in animalInfoDetail)
                    {
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Sj", item.Sj);
                        cmd.Parameters.AddWithValue("@WritngDe", item.WritngDe);
                        cmd.Parameters.AddWithValue("@Cn", item.Cn);
                        cmd.Parameters.AddWithValue("@Ty3Date", item.Ty3Date);
                        cmd.Parameters.AddWithValue("@Ty3Place", item.Ty3Place);
                        cmd.Parameters.AddWithValue("@Ty3Kind", item.Ty3Kind);
                        cmd.Parameters.AddWithValue("@Ty3Sex", item.Ty3Sex);
                        cmd.Parameters.AddWithValue("@Ty3Process", item.Ty3Process);
                        cmd.Parameters.AddWithValue("@Ty3Ingye", item.Ty3Ingye);
                        cmd.Parameters.AddWithValue("@Ty3Insu", item.Ty3Insu);
                        cmd.Parameters.AddWithValue("@Ty3Picture", item.Ty3Picture);

                        insRes += cmd.ExecuteNonQuery();
                    }
                    StsResult.Content = $"DB저장 {insRes}건 성공";
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB저장 오류! {ex.Message}");
            }
            #endregion

            SelectDB();
        }

        // 날짜 필터
        private async void PickDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PickDate.SelectedDate != null)
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    conn.Open();
                    var query = @"SELECT Idx,
                                     Sj,
                                     WritngDe,
                                     Cn,
                                     Ty3Date,
                                     Ty3Place,
                                     Ty3Kind,
                                     Ty3Sex,
                                     Ty3Process,
                                     Ty3Ingye,
                                     Ty3Insu,
                                     Ty3Picture
                                 FROM animalinfo
                                 WHERE WritngDe = @WritngDe";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@WritngDe", PickDate.SelectedDate.ToString()); // 콤보박스에서 선택한 날짜 파라미터 넣기
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "aminamlInfoDetail");
                    List<AminamlInfoDetail> aminamlInfoDetail = new List<AminamlInfoDetail>();

                    foreach (DataRow row in ds.Tables["aminamlInfoDetail"].Rows)
                    {
                        aminamlInfoDetail.Add(new AminamlInfoDetail
                        {
                            Idx = Convert.ToInt32(row["Idx"]),
                            Sj = Convert.ToString(row["sj"]),
                            WritngDe = Convert.ToDateTime(row["writngDe"]),
                            Cn = Convert.ToString(row["cn"]),
                            Ty3Date = Convert.ToString(row["ty3Date"]),
                            Ty3Place = Convert.ToString(row["ty3Place"]),
                            Ty3Kind = Convert.ToString(row["ty3Kind"]),
                            Ty3Sex = Convert.ToString(row["ty3Sex"]),
                            Ty3Process = Convert.ToString(row["ty3Process"]),
                            Ty3Ingye = Convert.ToString(row["ty3Ingye"]),
                            Ty3Insu = Convert.ToString(row["ty3Insu"]),
                            Ty3Picture = Convert.ToString(row["ty3Picture"])
                        });
                    }
                    this.DataContext = aminamlInfoDetail;
                    if(aminamlInfoDetail.Count == 0)
                    {
                        await Commons.ShowMessageAsync("정보", "선택한 날짜에 구조 건수가 없습니다.");
                    }
                    StsResult.Content = $"{PickDate.SelectedDate} {aminamlInfoDetail.Count}건 조회";


                }
            }
            else
            {
                SelectDB();
            }
        }

        // 그리드 특정 ROW 더블클릭하면 새창에 반려동물 상세정보
        private async void GrdResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var pet = GrdResult.SelectedItem as AminamlInfoDetail;
            if (pet != null)
            {
            var detailWindow = new DetailWindow(pet);
            detailWindow.Owner = this;
            detailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            detailWindow.ShowDialog();
            }
            else
            {
                await Commons.ShowMessageAsync("오류", "선택해주세요 ");

            }
        }
        
        // db select 기본
        public void SelectDB()
        {
            #region < DB 조회>
            using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                var query = @"SELECT Idx,
                                     Sj,
                                     WritngDe,
                                     Cn,
                                     Ty3Date,
                                     Ty3Place,
                                     Ty3Kind,
                                     Ty3Sex,
                                     Ty3Process,
                                     Ty3Ingye,
                                     Ty3Insu,
                                     Ty3Picture
                                 FROM animalinfo
                                 ORDER BY Idx DESC";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                var animalInfoDetail = new List<AminamlInfoDetail>();
                adapter.Fill(ds, "animalInfoDetail");
                foreach (DataRow row in ds.Tables["animalInfoDetail"].Rows)
                {
                    animalInfoDetail.Add(new AminamlInfoDetail
                    {
                        Idx = Convert.ToInt32(row["Idx"]),
                        Sj = Convert.ToString(row["sj"]),
                        WritngDe = Convert.ToDateTime(row["writngDe"]),
                        Cn = Convert.ToString(row["cn"]),
                        Ty3Date = Convert.ToString(row["ty3Date"]),
                        Ty3Place = Convert.ToString(row["ty3Place"]),
                        Ty3Kind = Convert.ToString(row["ty3Kind"]),
                        Ty3Sex = Convert.ToString(row["ty3Sex"]),
                        Ty3Process = Convert.ToString(row["ty3Process"]),
                        Ty3Ingye = Convert.ToString(row["ty3Ingye"]),
                        Ty3Insu = Convert.ToString(row["ty3Insu"]),
                        Ty3Picture = Convert.ToString(row["ty3Picture"])
                    });
                }

                this.DataContext = animalInfoDetail;
                StsResult.Content = $"{animalInfoDetail.Count} 건 조회";
            }
            #endregion
        }


        // 처리현황
        private void CboPrcs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboPrcs.SelectedValue != null)
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    conn.Open();
                    var query = @"SELECT Idx,
                                     Sj,
                                     WritngDe,
                                     Cn,
                                     Ty3Date,
                                     Ty3Place,
                                     Ty3Kind,
                                     Ty3Sex,
                                     Ty3Process,
                                     Ty3Ingye,
                                     Ty3Insu,
                                     Ty3Picture
                                 FROM animalinfo
                                 WHERE Ty3Process = @Ty3Process";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Ty3Process", CboPrcs.SelectedValue.ToString()); // 콤보박스에서 선택한 날짜 파라미터 넣기
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "aminamlInfoDetail");
                    List<AminamlInfoDetail> aminamlInfoDetail = new List<AminamlInfoDetail>();

                    foreach (DataRow row in ds.Tables["aminamlInfoDetail"].Rows)
                    {
                        aminamlInfoDetail.Add(new AminamlInfoDetail
                        {
                            Idx = Convert.ToInt32(row["Idx"]),
                            Sj = Convert.ToString(row["sj"]),
                            WritngDe = Convert.ToDateTime(row["writngDe"]),
                            Cn = Convert.ToString(row["cn"]),
                            Ty3Date = Convert.ToString(row["ty3Date"]),
                            Ty3Place = Convert.ToString(row["ty3Place"]),
                            Ty3Kind = Convert.ToString(row["ty3Kind"]),
                            Ty3Sex = Convert.ToString(row["ty3Sex"]),
                            Ty3Process = Convert.ToString(row["ty3Process"]),
                            Ty3Ingye = Convert.ToString(row["ty3Ingye"]),
                            Ty3Insu = Convert.ToString(row["ty3Insu"]),
                            Ty3Picture = Convert.ToString(row["ty3Picture"])
                        });
                    }
                    this.DataContext = aminamlInfoDetail;
                    StsResult.Content = $"{CboPrcs.SelectedValue} {aminamlInfoDetail.Count}건 조회";


                }
            }
            else
            {
                SelectDB();
            }
        }
    }
}
