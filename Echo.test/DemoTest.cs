using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class DemoTest
{
    [TestMethod]
    public void MyFirstUnitTest()
    {
        bool foo = false;
        
        Assert.IsFalse(foo);
        //Assert.Inconclusive("We need to write our first unit test");
    }
}