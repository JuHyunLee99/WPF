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

// 조회한거 사진기준으로 확인하고 삽입하기 해야함
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

        }

        //  실시간 조회
        private async void BtnReq_Click(object sender, RoutedEventArgs e)
        {
            string openApiUrl = "http://apis.data.go.kr/6260000/BusanPetAnimalInfoService/getPetAnimalInfo"; // URL
            openApiUrl += "?ServiceKey=" + "Hp7RL4tCw0cXBMTYsWCTrydbix%2Fqtqe4%2Bu5yRNze4LKbniVQhVKmNWMk8IxYObz6%2FEB41Vo47zCdEVUVRfAvsA%3D%3D"; // Service Key
            openApiUrl += "&pageNo=1";
            openApiUrl += "&numOfRows=450";
            openApiUrl += "&resultType=json";

            string result = string.Empty;

            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            // API REQ, RES
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

            var jsonResult = JObject.Parse(result);
            // Debug.WriteLine(jsonResult);

            var resultCode = Convert.ToInt32(jsonResult["getPetAnimalInfo"]["header"]["resultCode"]);

            var animalInfo = new List<AnimalInfo>();

            // JSON 처리
            try
            {
                if( resultCode == 0 ) // API 결과 정상이면 처리
                {
                    var data = jsonResult["getPetAnimalInfo"]["body"]["items"]["item"];
                    var json_arry = data as JArray;

                    
                    foreach (var item in json_arry )
                    {
                        animalInfo.Add(new AnimalInfo
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

            // DB 연결
            try
            {
                using(MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if(conn.State == ConnectionState.Closed) conn.Open();
                    var query = @"INSERT INTO animalinfo
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
                    foreach(AnimalInfo item in animalInfo)
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


        }

        // 반려동물 종류 필터
        private void CboPetType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        // 날짜 필터
        private void PickDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}
