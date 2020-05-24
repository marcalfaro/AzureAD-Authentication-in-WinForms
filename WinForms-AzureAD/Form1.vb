'###############
'#  
'#  Very simple example of how to Authenticate to your Azure Active Directory with as minimal lines as possible.
'#  
'#  Coded by Marc, 24 May 2020
'#  https://github.com/marcalfaro
'#
'###############

Option Explicit On
Option Strict On

Public Class Form1

    Private oAzureAD As New clsAzureAD

    'Specify your AzureAD ClientID and TenantID and build Public Client
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        oAzureAD.ClientId = ""  'Enter your ClientID here
        oAzureAD.TenantId = ""  'Enter your TenantID here
        oAzureAD.IdentityBuildClient()
    End Sub

    'Sign in to your Azure Active Directory
    Private Async Sub btnSignIn_Click(sender As Object, e As EventArgs) Handles btnSignIn.Click
        Dim rslt = Await oAzureAD.SignIn

        Dim rawToken As String = rslt.AccessToken
        Dim jwToken As String = oAzureAD.JWTokenToJsonString(rawToken, True)

        TextBox1.Text = rawToken
        TextBox2.Text = jwToken
    End Sub

    'Sign out your session
    Private Async Sub btnSignOut_Click(sender As Object, e As EventArgs) Handles btnSignOut.Click
        Await oAzureAD.SignOut
    End Sub

End Class

