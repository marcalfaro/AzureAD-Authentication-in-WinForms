Option Explicit On
Imports Microsoft.Identity.Client

Public Class Form1
    'reference: https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-v2-windows-desktop

    Public ClientId As String = "e3116694-7fe4-4fbd-9024-564a3688a776"
    Public Tenant As String = "80400424-236c-456d-9bf1-c90788c281f7"
    Public scopes As IEnumerable(Of String) = {"user.read"}

    Private PublicClientApp As IPublicClientApplication

    Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        PublicClientApp = PublicClientApplicationBuilder.Create(ClientId).WithRedirectUri("https://login.microsoftonline.com/common/oauth2/nativeclient").WithAuthority(AzureCloudInstance.AzurePublic, Tenant).Build()

        Dim accounts = Await PublicClientApp.GetAccountsAsync()
        If accounts.Any() Then
            Me.Label1.Text = "User has signed-in"
        Else
            Me.Label1.Text = "Sign in required"
        End If

    End Sub


    Private Async Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Await Login()
    End Sub

    Private Async Function Login() As Task(Of AuthenticationResult)
        Dim authResult As AuthenticationResult = Nothing

        Dim accounts = Await PublicClientApp.GetAccountsAsync()
        Dim firstAccount = accounts.FirstOrDefault()

        Dim noCachedAccount As Boolean = False

        Try
            authResult = Await PublicClientApp.AcquireTokenSilent(scopes, firstAccount).ExecuteAsync()
        Catch ex As MsalUiRequiredException
            Debug.WriteLine($"MsalUiRequiredException: {ex.Message}")
            noCachedAccount = True

        Catch ex As Exception
            Label1.Text = $"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}"
        End Try

        If authResult IsNot Nothing Then
            Debug.Print(authResult.Account.Username)
        Else
            If noCachedAccount Then
                Try
                    authResult = Await PublicClientApp.AcquireTokenInteractive(scopes).WithAccount(accounts.FirstOrDefault()).WithPrompt(Prompt.SelectAccount).ExecuteAsync()
                    Debug.Print(authResult.AccessToken)
                    Debug.Print(authResult.ToString)
                Catch msalex As MsalException
                    Label1.Text = $"Error Acquiring Token:{System.Environment.NewLine}{msalex}"
                End Try

            End If
        End If


        Return authResult
    End Function

    Private Async Function Logout() As Task
        Dim accounts = Await PublicClientApp.GetAccountsAsync()

        If accounts.Any() Then

            Try
                Await PublicClientApp.RemoveAsync(accounts.FirstOrDefault())
                Me.Label1.Text = "User has signed-out"

            Catch ex As MsalException
                Throw New Exception($"Error signing-out user: {ex.Message}")
            End Try
        End If
    End Function

    Private Async Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Await Logout()
    End Sub
End Class
