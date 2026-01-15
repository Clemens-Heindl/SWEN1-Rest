using Clemens.SWEN1.Handlers;
using Clemens.SWEN1.Server;
using Clemens.SWEN1.System;
using NUnit.Framework;
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Nodes;

namespace Clemens.SWEN1.Tests;
public class UnitTests
{

    private UserHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _handler = new UserHandler();
    }

    //User Tests
    [Test]
    public void TestHashing(){
        User user = new User();
        user.SetPassword("Hope");
        Assert.That(user.PasswordHash, Is.Not.EqualTo(string.Empty));
    }

    [Test]
    public void TestEmptyUsername()
    {
        var user = new User();

        var ex = Assert.Throws<ArgumentException>(() =>
        {
            user.UserName = "";
        });

        Assert.That(ex!.Message, Is.EqualTo("User name must not be empty."));
    }

    [Test]
    public void TestInValidToken()
    {

        var ex = Assert.Throws< UnauthorizedAccessException> (() =>
        {
            Session.verifyToken("xyz");
        });

        Assert.That(ex!.Message, Is.EqualTo("Invalid Session Token."));
    }

    [Test]
    public void TestEmptyCreator()
    {
        var entry = new MediaEntry();

        var ex = Assert.Throws<ArgumentException>(() =>
        {
            entry.Creator = "";
        });

        Assert.That(ex!.Message, Is.EqualTo("User name of creator must not be empty."));
    }

    [Test]
    public void TestEmptyOwner()
    {
        var rating = new Rating();

        var ex = Assert.Throws<ArgumentException>(() =>
        {
            rating.Owner = "";
        });

        Assert.That(ex!.Message, Is.EqualTo("User name of owner must not be empty."));
    }

    [Test]
    public void TestStarsCantBeMoreThan5()
    {
        var rating = new Rating();
        rating.Stars = 6;

        Assert.That(rating.Stars, Is.EqualTo(5));
    }

    [Test]
    public void TestServerCreation()
    {
        var server = new HttpRestServer();
        Assert.That(server, Is.Not.Null);
    }

    [Test]
    public void TestHandlerAttaching()
    {
        var server = new HttpRestServer();

        Assert.DoesNotThrow(() =>
        {
            server.RequestReceived += Handler.HandleEvent;
        });
    }

    [Test]
    public void TestHandlerDetaching()
    {
        var server = new HttpRestServer();

        server.RequestReceived += Handler.HandleEvent;

        Assert.DoesNotThrow(() =>
        {
            server.RequestReceived -= Handler.HandleEvent;
        });
    }

    [Test]
    public void TestMultipleHandlersCanBeAttached()
    {
        var server = new HttpRestServer();

        server.RequestReceived += Handler.HandleEvent;
        server.RequestReceived += Handler.HandleEvent;

        Assert.Pass();
    }

    [Test]
    public void TestPasswordHashChanges()
    {
        var user = new User();
        user.SetPassword("one");
        var hash1 = user.PasswordHash;

        user.SetPassword("two");
        var hash2 = user.PasswordHash;

        Assert.That(hash1, Is.Not.EqualTo(hash2));
    }

    [Test]
    public void TestSamePasswordProducesSameHash()
    {
        var user1 = new User();
        var user2 = new User();

        user1.SetPassword("secret");
        user2.SetPassword("secret");

        Assert.That(user1.PasswordHash, Is.EqualTo(user2.PasswordHash));
    }

    [Test]
    public void TestStarsCannotBeNegative()
    {
        var rating = new Rating();
        rating.Stars = -5;

        Assert.That(rating.Stars, Is.EqualTo(0));
    }

    [Test]
    public void TestYearCannotBeNegative()
    {
        var entry = new MediaEntry();
        entry.ReleaseYear = -5;

        Assert.That(entry.ReleaseYear, Is.EqualTo(1900));
    }

    [Test]
    public void TestNullToken()
    {
        Assert.Throws<UnauthorizedAccessException>(() =>
        {
            Session.verifyToken(null);
        });
    }

    [Test]
    public void TestJsonObjectCreation()
    {
        Assert.DoesNotThrow(() =>
        {
            var obj = new JsonObject
            {
                ["test"] = "value"
            };
        });
    }

    [Test]
    public void TestUserHandlerCreation()
    {
        var handler = new UserHandler();
        Assert.That(handler, Is.Not.Null);
    }

    [Test]
    public void TestMediaHandlerCreation()
    {
        var handler = new MediaHandler();
        Assert.That(handler, Is.Not.Null);
    }

    [Test]
    public void TestRatingHandlerCreation()
    {
        var handler = new RatingHandler();
        Assert.That(handler, Is.Not.Null);
    }

    [Test]
    public void TestUnauthorizedAccess()
    {
        var entry = new MediaEntry();

        Assert.Throws<UnauthorizedAccessException>(() =>
        {
            entry.Delete();
        });
    }
}
