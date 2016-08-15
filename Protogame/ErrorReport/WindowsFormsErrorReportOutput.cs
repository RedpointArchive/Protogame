#if PLATFORM_WINDOWS

using System;
using System.Threading;
using System.Windows.Forms;

namespace Protogame
{
    public class WindowsFormsErrorReportOutput : IErrorReportOutput
    {
        public void Report(Exception ex)
        {
            // Start Win Forms in a seperate thread so that it's responsive to user feedback.
            var thread = new Thread(() =>
            {
                string message;
                var aggregateException = ex as AggregateException;
                if (aggregateException != null)
                {
                    message = "---------------- ERROR LOG -----------------\r\n\r\n" +
                              ex.Message + "\r\n" + ex.StackTrace + "\r\n\r\n";
                    foreach (var exx in aggregateException.InnerExceptions)
                    {
                        message += exx.Message + "\r\n" + exx.StackTrace + "\r\n\r\n";
                    }

                    message += "--------------------------------------------\r\n";
                }
                else
                {
                    message = "---------------- ERROR LOG -----------------\r\n\r\n" +
                              ex.Message + "\r\n" + ex.StackTrace +
                              "\r\n\r\n--------------------------------------------\r\n";
                }

                var textArea = new System.Windows.Forms.TextBox();
                textArea.Multiline = true;
                textArea.Dock = DockStyle.Fill;
                textArea.Text = message;
                textArea.ScrollBars = ScrollBars.Vertical;

                var form = new System.Windows.Forms.Form();
                form.Width = Screen.PrimaryScreen.Bounds.Width / 2;
                form.Height = Screen.PrimaryScreen.Bounds.Height / 2;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.Controls.Add(textArea);
                form.Text = "An error has occurred.";

                Application.Run(form);
            });

            thread.Start();
            thread.Join();
        }
    }
}

#endif