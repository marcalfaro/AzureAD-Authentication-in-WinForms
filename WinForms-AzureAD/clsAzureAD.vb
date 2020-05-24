'###################################################################################
'# 
'#  Microsoft Authentication Library (Azure Active Directory Authentication)
'# 
'#  Register your app to Azure Portal under your organization
'#  Reference: https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-v2-windows-desktop
'# 
'#  Decode the Access Token received using Microsoft's own JSON Web Token parser: https://jwt.ms/
'# 
'#  Coded by Marc
'#  24 May 2020
'# 
'###################################################################################

Option Explicit On

Imports System.IdentityModel.Tokens.Jwt
Imports Microsoft.Identity.Client
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class clsAzureAD

    'Note: Register your app to Azure Portal under your organization
    'reference: https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-v2-windows-desktop

    Public Property ClientId As String
    Public Property TenantId As String

    Private Scopes As IEnumerable(Of String) = {"user.read"}

    Private PublicClientApp As IPublicClientApplication

    Public Sub AzureBuildClient()
        PublicClientApp = PublicClientApplicationBuilder.Create(ClientId) _
            .WithRedirectUri("https://login.microsoftonline.com/common/oauth2/nativeclient") _
            .WithAuthority(AzureCloudInstance.AzurePublic, TenantId) _
            .Build()
    End Sub

    Public Async Function HasSignedIn() As Task(Of Boolean)
        Dim accounts = Await PublicClientApp.GetAccountsAsync()
        If accounts.Any() Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Async Function SignIn() As Task(Of AuthenticationResult)
        Dim authResult As AuthenticationResult = Nothing

        Dim accounts = Await PublicClientApp.GetAccountsAsync()
        Dim firstAccount = accounts.FirstOrDefault()

        Dim needsSignIn As Boolean = False
        Try
            authResult = Await PublicClientApp.AcquireTokenSilent(Scopes, firstAccount).ExecuteAsync()
        Catch ex As MsalUiRequiredException
            Debug.WriteLine($"MsalUiRequiredException: {ex.Message}")
            needsSignIn = True

        Catch ex As Exception
            Debug.Print($"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}")
        End Try

        If authResult IsNot Nothing Then
            Debug.Print(authResult.Account.Username)
        Else
            If needsSignIn Then
                Try
                    authResult = Await PublicClientApp.AcquireTokenInteractive(Scopes).WithAccount(accounts.FirstOrDefault()).WithPrompt(Prompt.SelectAccount).ExecuteAsync()
                    Debug.Print(authResult.AccessToken)
                    Debug.Print(authResult.ToString)
                Catch msalex As MsalException
                    Debug.Print($"Error Acquiring Token:{System.Environment.NewLine}{msalex}")
                End Try
            End If
        End If

        Return authResult
    End Function

    Public Async Function SignOut() As Task
        Dim accounts = Await PublicClientApp.GetAccountsAsync()

        If accounts.Any() Then
            Try
                Await PublicClientApp.RemoveAsync(accounts.FirstOrDefault())
            Catch ex As MsalException
                Throw New Exception($"Error signing-out user: {ex.Message}")
            End Try
        End If
    End Function


    Public Function JWTokenToJsonString(ByVal accessToken As String, Optional ByVal Prettify As Boolean = False) As String
        Dim rslt As String = String.Empty
        Try
            Debug.Print(accessToken)

            If Not String.IsNullOrWhiteSpace(accessToken) Then
                Dim jwtTokenHandler = New JwtSecurityTokenHandler
                Dim jwToken As JwtSecurityToken = jwtTokenHandler.ReadJwtToken(accessToken)

                'Dim _aud As String = jwToken.Audiences.FirstOrDefault
                'Console.WriteLine("email => " & jwToken.Claims.FirstOrDefault(Function(c) c.Type = "email").Value)

                Debug.Print(jwToken.ToString)

                If Not Prettify Then
                    rslt = jwToken.ToString
                Else
                    Dim jsonStrings As String() = Split(jwToken.ToString, "}.{")
                    jsonStrings(0) = PrettifyJSON(jsonStrings(0) & "}")
                    jsonStrings(1) = PrettifyJSON("{" & jsonStrings(1))

                    rslt = $"{jsonStrings(0)}{vbNewLine}.{vbNewLine}{jsonStrings(1)}"
                End If

            End If
        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try
        Return rslt
    End Function

    Public Function PrettifyJSON(ByVal jsonString As String) As String
        Dim rslt As String = jsonString
        Try
            rslt = JToken.Parse(jsonString).ToString(Formatting.Indented)
        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try
        Return rslt
    End Function

End Class
