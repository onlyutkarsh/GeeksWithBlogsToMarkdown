using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using GeeksWithBlogsToMarkdown.ViewModels;
using NUnit.Framework;

namespace GeeksWithBlogsToMarkdown.Tests
{
    [TestFixture]
    public class MainViewModelTests
    {
        [Test]
        public void SavePostTest()
        {
            var mainViewModel = new MainWindowViewModel();
            mainViewModel.HtmlMarkup = "<p>I had <a href=\"http://geekswithblogs.net/onlyutkarsh/archive/2013/06/02/loading-custom-assemblies-in-visual-studio-extensions-again.aspx\" target=\"_blank\">previously</a> written on how to load custom assemblies in your extension using <code>AppDomain.CurrentDomain.AssemblyResolve</code>. It required few lines of code to be written in your VS package class. Today I am going to show you an easier way of doing the same. </p>  <p>Visual Studio provides <code><a href=\"http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.providebindingpathattribute.aspx\" target=\"_blank\">ProvideBindingPath</a></code> attribute which lets Visual Studio know other paths from where your extension loads the assemblies. The usage of this attribute is very simple, you just need to decorate your package class with it. </p>  <pre class=\"prettyprint\">[Guid(GuidList.guidmin2015PkgString)]\r\n[ProvideBindingPath]\r\npublic sealed class min2015Package : Package\r\n{\r\n}</pre>\r\n\r\n<p>Once you do that and compile your extension, the &lt;extension&gt;.pkgdef file will be modified to include a new line something like below, where {PackageGuid} will be the guid of your package.</p>";
            mainViewModel.OnSavePost();
        }
    }
}
