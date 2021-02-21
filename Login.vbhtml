@* Remove this section if you are using bundling *@
@Section Scripts
    <script src="~/Scripts/jquery.validate.min.js"></script>
    <script src="~/Scripts/jquery.validate.unobtrusive.min.js"></script>
End Section

@Code
    Layout = "~/_SiteLayout.vbhtml"
    PageData("Title") = "Log in"

    ' Initialize general page variables
    Dim email As String = ""
    Dim password As String = ""
    Dim rememberMe As Boolean = False

    Dim returnUrl As String = Request.QueryString("ReturnUrl")
    If returnUrl.IsEmpty() Then
        ' Some external login providers always require a return URL value
        returnUrl = Href("~/")
    End If

    ' Setup validation
    Validation.RequireField("email", "You must specify an email address.")
    Validation.RequireField("password", "You must specify a password.")
    Validation.Add("password",
        Validator.StringLength(
            maxLength:=Int32.MaxValue,
            minLength:=6,
            errorMessage:="Password must be at least 6 characters"))

    ' If this is a POST request, validate and process data
    If IsPost Then
        AntiForgery.Validate()
        ' is this an external login request?
        Dim provider As String = Request.Form("provider")
        If Not Provider.IsEmpty() Then
            OAuthWebSecurity.RequestAuthentication(Provider, Href("~/Account/RegisterService", New With { .ReturnUrl = returnUrl }))
            Return
        ElseIf Validation.IsValid() Then
            email = Request.Form("email")
            password = Request.Form("password")
            rememberMe = Request.Form("rememberMe").AsBool()

            If WebSecurity.UserExists(email) AndAlso WebSecurity.GetPasswordFailuresSinceLastSuccess(email) > 4 AndAlso WebSecurity.GetLastPasswordFailureDate(email).AddSeconds(60) > DateTime.UtcNow Then
                Response.Redirect("~/Account/AccountLockedOut")
                Return
            End If

            ' Attempt to log in using provided credentials
            If WebSecurity.Login(email, password, rememberMe) Then
                Context.RedirectLocal(returnUrl)
                Return
            Else
                ModelState.AddFormError("The user name or password provided is incorrect.")
            End If
        End If
    End If
End Code

<hgroup class="title">
  
</hgroup>
<div id="logo">
    <h1><img src="./jimg/icon.ico" width="40" height="40" /><i>LOG IN PORTAL </i></h1>
</div> 
<section class="gov-login">
    <div id="fade-box">
        <form method="post">
            @AntiForgery.GetHtml()
            @* If one or more validation errors exist, show an error *@
            @Html.ValidationSummary("Please Register With Our Portal Team", excludeFieldErrors:=True, htmlAttributes:=Nothing)
            <gov-login form>
                <li class="email">
                    <label for="email" @If Not ModelState.IsValidField("email") Then @<text> class="error-label" </text>    End If></label>
                    <input type="text" id="email" name="email" value="@email" @Validation.For("email") />
                    @* Write any user name validation errors to the page *@
                    @Html.ValidationMessage("email")
                </li>
                <li class="password">
                    <label for="password" @If Not ModelState.IsValidField("password") Then @<text> class="error-label" </text>    End If></label>
                    <input type="password" id="password" name="password" @Validation.For("password") />
                    @* Write any password validation errors to the page *@
                    @Html.ValidationMessage("password")
              
                </li>
            </gov-login>
            <input type="submit" value="Log in" />
            </fieldset>
        </form>

</section>

<section class="social" id="socialLoginForm">
</section>
