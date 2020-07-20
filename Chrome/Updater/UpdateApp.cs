using AutoUpdaterDotNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms; 

namespace Chrome.Updater
{
    class UpdateApp
    { 
        private void AutoUpdater_ApplicationExitEvent()
        {
            Thread.Sleep(5000);
            Application.Exit();
        } 
        private void AutoUpdaterOnParseUpdateInfoEvent(ParseUpdateInfoEventArgs args)
        {
            dynamic json = JsonConvert.DeserializeObject(args.RemoteData);
            args.UpdateInfo = new UpdateInfoEventArgs
            {
                CurrentVersion = json.version,
                ChangelogURL = json.changelog,
                DownloadURL = json.url,
                Mandatory = new Mandatory
                {
                    Value = json.mandatory.value,
                    UpdateMode = json.mandatory.mode,
                    MinimumVersion = json.mandatory.minVersion
                },
                CheckSum = new CheckSum
                {
                    Value = json.checksum.value,
                    HashingAlgorithm = json.checksum.hashingAlgorithm
                }
            };
        }
        DialogResult dialogResult;
        void Message(UpdateInfoEventArgs args)
        {
            
            if (args.Mandatory.Value)
            {
                dialogResult =
                    MessageBox.Show(
                         $@"Yeni bir versiyon {args.CurrentVersion} bulunmakta. Suanda  {
                                args.InstalledVersion
                            } versiyonunu kullanmaktasiniz. Simdi guncellemek istermisiniz?",
                        @"Update Available",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
            }
            else
            {
                dialogResult =
                    MessageBox.Show(
                        $@"Yeni bir versiyon {args.CurrentVersion} bulunmakta. Suanda  {
                                args.InstalledVersion
                            } versiyonunu kullanmaktasiniz. Simdi guncellemek istermisiniz?", @"Update Available",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);
            }
            if (dialogResult.Equals(DialogResult.Yes) || dialogResult.Equals(DialogResult.OK))
            {
                try
                {
                    //You can use Download Update dialog used by AutoUpdater.NET to download the update.

                    if (AutoUpdater.DownloadUpdate(args))
                    {
                        Application.Exit();
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args != null)
            {
                if (args.IsUpdateAvailable)
                { 
                    Message(args);

                   
                }
                else
                {
                    MessageBox.Show(@"Şuanda bir güncelleme bulunmamaktadır. Lütfen daha sonra tekrar deneyiniz.", @"Update Bulunamadı",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show(
                    @"Güncelleme aranırken sorun oluştu. Lütfen internet bağlantınızı kontrol edip tekrar deneyiniz.",
                    @"Update Kontrol Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
