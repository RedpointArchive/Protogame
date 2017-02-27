using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Prototest.Library.Version11;

namespace Protogame.Tests
{
    public class GraphicalHtmlSummaryConsumer : ITestSummaryConsumer
    {
        public void HandleResults(List<TestResult> results)
        {
            var resultsWithAttachments = results.Where(x => x.Attachments.Count > 0).ToList();

            var content = @"
<!DOCTYPE html>
<html lang=""en"">
<head>
<link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css"" integrity=""sha384-1q8mTJOASx8j1Au+a5WDVnPi2lkFfwwEAa8hDDdjZlpLegxhjVME1fgjWPGmkzs7"" crossorigin=""anonymous"">
<link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/font-awesome/4.5.0/css/font-awesome.min.css"">
<style type=""text/css"">
#container { max-width: 800px; margin: auto; }
.mcol { text-align: center; width:0px; width: 47px; padding-left: 0px; padding-right: 0px; }
.timestamp { width: 160px; }
.commit { width: 100px; }
</style>
</head>
<body>
<div id=""container-fluid"" style=""margin-left: 2em; margin-right: 2em;"">
<h1>Visual Test Results</h1>
<table class=""table""><tr><th colspan=""2"">Test</th><th>Comparison</th><th>Expected</th><th>Actual</th><th>Threshold</th><th>Measured</th></tr>";

            foreach (var ra in resultsWithAttachments)
            {
                var keys = ra.Attachments.Keys.Where(x => x.StartsWith("expected-"))
                    .Select(x => x.Substring("expected-".Length));
                foreach (var k in keys)
                {
                    var threshold = (double)ra.Attachments["threshold-" + k];
                    var measured = (double)ra.Attachments["measured-" + k];
                    var color = measured < threshold ? "red" : "lime";

                    content += "<tr>";
                    content += "<td style=\"background-color: " + color + "; width: 20px;\">&nbsp;</td>";
                    content += "<td>" + ra.Entry.TestClass.Name + ":" + ra.Entry.TestMethod.Name + "</td>";
                    content += "<td>" + ra.Attachments["name-" + k] + "</td>";
                    content += "<td><img src=\"data:image/png;base64," +
                               GetBase64PngFromStream((Stream) ra.Attachments["expected-" + k]) +
                               "\" style=\"max-width: 300px;\" /></td>";
                    content += "<td><img src=\"data:image/png;base64," +
                               GetBase64PngFromStream((Stream)ra.Attachments["actual-" + k]) +
                               "\" style=\"max-width: 300px;\" /></td>";
                    content += "<td>" + threshold + "%</td>";
                    content += "<td>" + measured + "%</td>";
                    content += "</tr>";
                }
            }

            content += "</table></div></body></html>";

            Directory.CreateDirectory("VisualHTML");

            using (var writer = new StreamWriter(Path.Combine("VisualHTML", "index.html")))
            {
                writer.Write(content);
            }
        }

        private string GetBase64PngFromStream(Stream raAttachment)
        {
            raAttachment.Seek(0, SeekOrigin.Begin);
            var bytes = new byte[raAttachment.Length];
            raAttachment.Read(bytes, 0, bytes.Length);
            raAttachment.Seek(0, SeekOrigin.Begin);
            return Convert.ToBase64String(bytes);
        }
    }
}
