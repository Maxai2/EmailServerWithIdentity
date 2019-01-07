$(document).ready(function () {
    if (localStorage["access_token"])
    {
        $("#logInForm").hide();
        $("#accountInfo").append("<p>Login: " + localStorage["login"] + "</p>");
        $("#accountInfo").append("<button onclick='logOut()'>Log Out</button>");
    }
    else {}
});

function LogIn()
{
    event.preventDefault();
    
    $.ajax({
        url: "http://localhost:60252/api/account/login",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify(
            {
                "login": $("#logInForm #login").val(),
                "password": $("#logInForm #password").val(),
            }),
        success: function (data)
        {
            localStorage["access_token"] = data.accessToken;
            localStorage["refresh_token"] = data.refreshToken;
            localStorage["login"] = data.login;
            $("#logInForm").hide();
            $("#accountInfo").append("<p>Login: " + localStorage["login"] + "</p>");
            $("#accountInfo").append("<button onclick='logOut()'>Log Out</button>");
        },
        error: function(data)
        {
            alert('error');
        }
    });
}

function logOut()
{
    $.ajax({
        url: "http://localhost:60252/api/account/logout",
        method: "GET",
        contentType: "application/json",
        headers:
        {
            "Authorization": "Bearer " + localStorage["access_token"]
        },
        complete: function()
        {
            console.log("exit");
            localStorage.clear();
            console.log(localStorage);
            window.location.reload();
        }
    });
}

function getAllBooks() {
    $.ajax({
        url: "http://localhost:60252/api/book",
        method: "GET",
        contentType: "application/json",
        success: function (books) {
            $("#books tbody").empty();
            $.each(books, function (index, book) {
                $("#books tbody").append("<tr><td>" + book.id + "</td><td>" + book.title + "</td><td>" + book.year
                    + "</td><td><button onclick='editBook(" + book.id + ")'>edit</button></td>"
                    + "<td><button onclick='deleteBook(" + book.id + ")'>delete</button></td></tr>");
            })
        }
    });
}

function editBook(id) {
    $.ajax({
        url: "http://localhost:60252/api/book/" + id,
        method: "GET",
        contentType: "application/json",
        success: function (book) {
            $("#id").val(book.id);
            $("#title").val(book.title);
            $("#year").val(book.year);
        }
    });
}

function submitBookForm() {
    event.preventDefault();

    if ($("#id").val() != '') {
        $.ajax({
            url: "http://localhost:60252/api/book",
            method: "PUT",
            contentType: "application/json",
            data: JSON.stringify({
                id: $("#id").val(),
                title: $("#title").val(),
                year: $("#year").val(),
            }),
            success: function (book) {
                $("#id").val("");
                $("#title").val("");
                $("#year").val("");
                getAllBooks();
            },
            statusCode: {
                401: function () {
                    console.log("access token dead!");
                    updateToken(submitBookForm);
                }
            }
        });
    }
    else {
        $.ajax({
            url: "http://localhost:60252/api/book",
            method: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                title: $("#title").val(),
                year: $("#year").val(),
            }),
            success: function (book) {
                $("#errors").empty();
                $("#id").val("");
                $("#title").val("");
                $("#year").val("");
                getAllBooks();
            },
            statusCode: {
                401: function () {
                    console.log("access token dead!");
                    updateToken(submitBookForm);
                }
            }
        });
    }
}

function updateToken(callback) { // updateToken(editBook)
    $.ajax({
        url: "http://localhost:60252/api/account/token",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify(localStorage["refresh_token"]),
        success: function (data) {
            localStorage["access_token"] = data.accessToken;
            localStorage["refresh_token"] = data.refreshToken;
            localStorage["login"] = data.login;
            callback();
        },
        error: function () {
            localStorage.clear();
            window.location.reload();
        }
    });
}