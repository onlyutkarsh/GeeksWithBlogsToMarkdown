using System;
using System.Threading;
using System.Windows;

namespace GeeksWithBlogsToMarkdown.Common
{
    internal class ClipboardHelper : StaHelper
    {
        private readonly object _data;
        private readonly string _format;

        public ClipboardHelper(string format, object data)
        {
            _format = format;
            _data = data;
        }

        protected override void Work()
        {
            var obj = new System.Windows.Forms.DataObject(
                _format,
                _data
            );

            Clipboard.SetDataObject(obj, true);
        }
    }

    internal abstract class StaHelper
    {
        private readonly ManualResetEvent _complete = new ManualResetEvent(false);

        public bool DontRetryWorkOnFailed { get; set; }

        public void Go()
        {
            var thread = new Thread(new ThreadStart(DoWork))
            {
                IsBackground = true,
            };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        // Implemented in base class to do actual work.
        protected abstract void Work();

        // Thread entry method
        private void DoWork()
        {
            try
            {
                _complete.Reset();
                Work();
            }
            catch (Exception)
            {
                if (DontRetryWorkOnFailed)
                    throw;
                else
                {
                    try
                    {
                        Thread.Sleep(1000);
                        Work();
                    }
                    catch
                    {
                        // ex from first exception
                    }
                }
            }
            finally
            {
                _complete.Set();
            }
        }
    }
}