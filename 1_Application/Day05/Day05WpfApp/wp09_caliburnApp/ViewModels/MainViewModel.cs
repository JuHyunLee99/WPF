using Caliburn.Micro;
using MahApps.Metro.Converters;
using wp09_caliburnApp.Models;

namespace wp09_caliburnApp.ViewModels
{
    public class MainViewModel : Screen
    {
        private string firstName = "Juhyun";

        // Caliburn version업으로 변경
        public string FirstName
        {
            get => firstName;
            set
            {
                firstName = value;
                NotifyOfPropertyChange(() => FirstName);  //속성값 변경될때
                NotifyOfPropertyChange(nameof(CanClearName));
                NotifyOfPropertyChange(nameof(FullName));
            }
        }

        private string lastName = "Lee";

        public string LastName
        {
            get => lastName;
            set 
            {
                lastName = value;
                NotifyOfPropertyChange(() => LastName);
                NotifyOfPropertyChange(nameof(CanClearName));
                NotifyOfPropertyChange(nameof(FullName));
            }
        }
        public string FullName
        {
            get => $"{LastName} {FirstName}";
        }

        // 콤보박스에 바인딩할 속성
        // 이런곳에는 var를 쓸 수 없음.
        private BindableCollection<Person> managers = new BindableCollection<Person> ();

        public BindableCollection<Person> Managers
        {
            get => managers;
            set => managers = value; 
        }

        // 콤보박스에 선택된 값을 지정할 속성
        private Person selectedManager;

        public Person SelectedManager
        {
            get => selectedManager;
            set
            { 
                selectedManager = value;
                LastName = selectedManager.LastName;
                FirstName = selectedManager.FisrtName;
                NotifyOfPropertyChange(nameof(SelectedManager));
            } 
        }

        public MainViewModel()
        {
            // DB를 사용하면 여기서 DB접속 > 데이터 Select까지...
            Managers.Add(new Person { FisrtName = "John", LastName = "Carmack" });
            Managers.Add(new Person { FisrtName = "Steve", LastName = "Jobs" });
            Managers.Add(new Person { FisrtName = "Bill", LastName = "Gates" });
            Managers.Add(new Person { FisrtName = "Elon", LastName = "Musk" });
        }

        public void ClearName()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
        }

        // 메서드와 이름동일하게 하고 앞에 Can을 붙임
        public bool CanClearName
        {
            get => !(string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName));
        }
    }
}
