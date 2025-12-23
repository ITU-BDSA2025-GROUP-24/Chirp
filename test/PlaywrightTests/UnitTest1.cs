using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class HomepageTests : PageTest
{
    [Test]
    public async Task HomepageLoads()
    {
        await Page.GotoAsync("https://bdsagroup24chirpremotedb-chdgcrfub5b7bbb5.germanywestcentral-01.azurewebsites.net/");
        await Expect(Page).ToHaveURLAsync(new Regex("azurewebsites"));
    }
}