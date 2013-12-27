using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VKStatistic
{
    /// <summary>
    /// Логика взаимодействия для dialogs.xaml
    /// </summary>
    public partial class dialogs : Window
    {
        public dialogs()
        {
            InitializeComponent();
        }
        public override void EndInit()
        {
            base.EndInit();

            authorisationpage.Navigate("https://oauth.vk.com/authorize?client_id=3072479&scope=messages,friends&redirect_uri=vk.com&display=page&v=5.2&response_type=token");
            authorisationpage.Navigated += authorisationpage_Navigated;
        }

        private void authorisationpage_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            string currenturl = e.Uri.ToString();
            if(currenturl.Contains("access_token"))
            {
                var a = currenturl.SkipWhile(x=>x!='=').Skip(1).TakeWhile(x => x != '&').ToArray();

                foreach (var c in a)
                {
                    Server.access_token += c;
                }
            }
            Close();
        }
        protected override void OnClosed(EventArgs e)
        {
            var a = authorisationpage.Source.Fragment.SkipWhile(x=>x!='=').Skip(1).TakeWhile(x => x != '&').ToArray();
            foreach (var c in a)
            {
                Server.access_token += c;
            }
            base.OnClosed(e);
        }

    }
}
