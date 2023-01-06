using EsapiEssentials.Plugin;
using AutoFlash;
using VMS.TPS.Common.Model.API;
using System.Windows;

[assembly: ESAPIScript(IsWriteable = true)]

namespace VMS.TPS
{
    public class Script : ScriptBase
    {
        public override void Run(PluginScriptContext context)
        {
            if (context.Patient != null)
            {
                var esapiService = new EsapiService(context);

                using (var ui = new UiRunner())
                {
                    ui.Run(() =>
                    {
                        var window = new MainWindow();
                        var dialogService = new DialogService(window);
                        var viewModel = new MainViewModel(esapiService, dialogService);
                        window.DataContext = viewModel;
                        window.ShowDialog();
                    });
                }
            }
            else
            {
                MessageBox.Show("Please open a patient.");
            }
        }
    }
}

