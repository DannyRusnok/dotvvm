﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotvvm.Samples.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Riganti.Utils.Testing.Selenium.Core;

namespace DotVVM.Samples.Tests.Control
{
    [TestClass]
    public class ContentPlaceHolderTests : SeleniumTest
    {
        [TestMethod]
        public void Control_ContentPlaceHolder_ContentPlaceHolderPage()
        {
            RunInAllBrowsers(browser =>
            {
                browser.NavigateToUrl(SamplesRouteUrls.ControlSamples_ContentPlaceHolder_ContentPlaceHolderPage);
                browser.First("#innerHtmlTest").CheckIfJsPropertyInnerHtml(html => string.IsNullOrWhiteSpace(System.Net.WebUtility.HtmlDecode(html)), "Inner html has to be empty.");
            });
        }

        [TestMethod]
        public void Control_ContentPlaceHolder_ContentPlaceHolderPage_ContentTest()
        {
            RunInAllBrowsers(browser =>
            {
                browser.NavigateToUrl(SamplesRouteUrls.ControlSamples_ContentPlaceHolder_ContentPlaceHolderPage_ContentTest);
                browser.First("#innerHtmlTest").CheckIfJsPropertyInnerHtml(html => !string.IsNullOrWhiteSpace(System.Net.WebUtility.HtmlDecode(html)), "Inner html has to contain specified content.");
            });
        }

    }
}
