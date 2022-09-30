using System;
using Xunit;
using Villedepommes;

public class SqlEngineTests
{
    [Fact]
    public void TestTablePeople()
    {
        var tp = new TablePeople();
        var r = tp.next();
        Assert.NotNull(r);
        Assert.Equal("1", r[0]);
        Assert.Equal("Nick", r[1]);
        r = tp.next();
        Assert.NotNull(r);
        Assert.Equal("Kelly", r[1]);
        Assert.Equal("2", r[0]);
        tp.reset();
        Assert.NotNull(r);
        r = tp.next();
        Assert.Equal("1", r[0]);
        Assert.Equal("Nick", r[1]);
        return;
    }

    [Fact]
    public void TestSimpleJoin()
    {
        return;
    }
}