Imports System.Data.SqlClient
Imports System.Web.Services

Partial Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadColorsData()
        End If
    End Sub

    Private Sub LoadColorsData()
        Dim connectionString As String = System.Configuration.ConfigurationManager.ConnectionStrings("ColorsDBConnection").ConnectionString
        Using conn As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand("SELECT * FROM Colors ORDER BY DisplayOrder", conn)
            Dim adapter As New SqlDataAdapter(cmd)
            Dim dt As New DataTable()
            conn.Open()
            adapter.Fill(dt)
            conn.Close()

            ColorsGridView.DataSource = dt
            ColorsGridView.DataBind()
        End Using
    End Sub

    Protected Sub ColorsGridView_RowCancelingEdit(ByVal sender As Object, ByVal e As GridViewCancelEditEventArgs)
        ColorsGridView.EditIndex = -1
        LoadColorsData()
    End Sub

    Protected Sub ColorsGridView_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)
        Dim id As Integer = Convert.ToInt32(ColorsGridView.DataKeys(e.RowIndex).Value)
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("ColorsDBConnection").ConnectionString

        Using conn As New SqlConnection(connectionString)
            conn.Open()

            Dim deleteCmd As New SqlCommand("DELETE FROM Colors WHERE Id=@Id", conn)
            deleteCmd.Parameters.AddWithValue("@Id", id)
            deleteCmd.ExecuteNonQuery()

            Dim reorderCmd As New SqlCommand("WITH CTE AS (SELECT Id, ROW_NUMBER() OVER (ORDER BY DisplayOrder) AS NewOrder FROM Colors) UPDATE Colors SET DisplayOrder = NewOrder FROM CTE WHERE Colors.Id = CTE.Id", conn)
            reorderCmd.ExecuteNonQuery()
        End Using

        LoadColorsData()
    End Sub

    Protected Sub btnAddNewColor_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim colorName As String = txtColorName.Text
        Dim price As Integer
        Dim displayOrder As Integer

        If Not Integer.TryParse(txtPrice.Text, price) Then Exit Sub
        If Not Integer.TryParse(txtDisplayOrder.Text, displayOrder) Then Exit Sub

        Dim colorCode As String = txtColorCode.Text
        Dim inStock As Boolean = chkInStock.Checked

        Dim connectionString As String = System.Configuration.ConfigurationManager.ConnectionStrings("ColorsDBConnection").ConnectionString
        Using conn As New SqlConnection(connectionString)
            conn.Open()

            Dim checkCmd As New SqlCommand("SELECT COUNT(*) FROM Colors WHERE DisplayOrder = @DisplayOrder", conn)
            checkCmd.Parameters.AddWithValue("@DisplayOrder", displayOrder)
            Dim exists As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())

            If exists > 0 Then
                Dim shiftCmd As New SqlCommand("UPDATE Colors SET DisplayOrder = DisplayOrder + 1 WHERE DisplayOrder >= @DisplayOrder", conn)
                shiftCmd.Parameters.AddWithValue("@DisplayOrder", displayOrder)
                shiftCmd.ExecuteNonQuery()
            End If

            Dim insertCmd As New SqlCommand("INSERT INTO Colors (ColorName, Price, DisplayOrder, InStock, ColorCode) VALUES (@ColorName, @Price, @DisplayOrder, @InStock, @ColorCode)", conn)
            insertCmd.Parameters.AddWithValue("@ColorName", colorName)
            insertCmd.Parameters.AddWithValue("@Price", price)
            insertCmd.Parameters.AddWithValue("@DisplayOrder", displayOrder)
            insertCmd.Parameters.AddWithValue("@InStock", inStock)
            insertCmd.Parameters.AddWithValue("@ColorCode", colorCode)
            insertCmd.ExecuteNonQuery()
        End Using

        txtColorName.Text = String.Empty
        txtPrice.Text = String.Empty
        txtDisplayOrder.Text = String.Empty
        txtColorCode.Text = "#000000"
        chkInStock.Checked = False

        ColorsGridView.EditIndex = -1
        LoadColorsData()
    End Sub

    Protected Sub ColorsGridView_RowEditing(ByVal sender As Object, ByVal e As GridViewEditEventArgs)
        ColorsGridView.EditIndex = e.NewEditIndex
        LoadColorsData()
    End Sub

    Protected Sub ColorsGridView_RowUpdating(ByVal sender As Object, ByVal e As GridViewUpdateEventArgs)
        Try
            Dim id As Integer = Convert.ToInt32(ColorsGridView.DataKeys(e.RowIndex).Value.ToString())

            Dim row As GridViewRow = ColorsGridView.Rows(e.RowIndex)

            Dim colorNameControl As TextBox = CType(row.FindControl("txtColorNameEdit"), TextBox)
            Dim priceTextBox As TextBox = CType(row.FindControl("txtPriceEdit"), TextBox)
            Dim displayOrderTextBox As TextBox = CType(row.FindControl("txtDisplayOrderEdit"), TextBox)
            Dim colorCodeTextBox As TextBox = CType(row.FindControl("txtColorCodeEdit"), TextBox)
            Dim inStockCheckBox As CheckBox = CType(row.FindControl("chkInStockEdit"), CheckBox)

            If colorNameControl Is Nothing OrElse priceTextBox Is Nothing OrElse displayOrderTextBox Is Nothing OrElse colorCodeTextBox Is Nothing OrElse inStockCheckBox Is Nothing Then
                Throw New NullReferenceException("אחד מהשדות אינו קיים בטבלה.")
            End If

            Dim colorName As String = colorNameControl.Text
            Dim price As Integer = Convert.ToInt32(priceTextBox.Text)
            Dim displayOrder As Integer = Convert.ToInt32(displayOrderTextBox.Text)
            Dim colorCode As String = colorCodeTextBox.Text
            Dim inStock As Boolean = inStockCheckBox.Checked

            Dim connectionString As String = System.Configuration.ConfigurationManager.ConnectionStrings("ColorsDBConnection").ConnectionString
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Dim cmd As New SqlCommand("UPDATE Colors SET ColorName=@ColorName, Price=@Price, DisplayOrder=@DisplayOrder, InStock=@InStock, ColorCode=@ColorCode WHERE Id=@Id", conn)
                cmd.Parameters.AddWithValue("@Id", id)
                cmd.Parameters.AddWithValue("@ColorName", colorName)
                cmd.Parameters.AddWithValue("@Price", price)
                cmd.Parameters.AddWithValue("@DisplayOrder", displayOrder)
                cmd.Parameters.AddWithValue("@InStock", inStock)
                cmd.Parameters.AddWithValue("@ColorCode", colorCode)
                cmd.ExecuteNonQuery()
            End Using

            ColorsGridView.EditIndex = -1
            LoadColorsData()
        Catch ex As Exception
            Response.Write("שגיאה במהלך עדכון הנתונים: " & ex.Message)
        End Try
    End Sub

    Protected Sub ColorsGridView_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim colorCode As String = DataBinder.Eval(e.Row.DataItem, "ColorCode").ToString()
            e.Row.Cells(4).BackColor = System.Drawing.ColorTranslator.FromHtml(colorCode)
            e.Row.Cells(4).Text = colorCode
        End If
    End Sub

    Private Sub ReorderDisplayOrder()
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("ColorsDBConnection").ConnectionString
        Using conn As New SqlConnection(connectionString)
            conn.Open()
            Dim cmd As New SqlCommand("UPDATE Colors SET DisplayOrder = Id", conn)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    <WebMethod>
    Public Shared Function UpdateDisplayOrder(orderData As List(Of DisplayOrderItem)) As Boolean
        Try
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("ColorsDBConnection").ConnectionString
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Using transaction As SqlTransaction = conn.BeginTransaction()
                    Try
                        For Each item In orderData
                            Using cmd As New SqlCommand("UPDATE Colors SET DisplayOrder = @DisplayOrder WHERE Id = @Id", conn, transaction)
                                cmd.Parameters.AddWithValue("@DisplayOrder", item.DisplayOrder)
                                cmd.Parameters.AddWithValue("@Id", item.Id)
                                cmd.ExecuteNonQuery()
                            End Using
                        Next
                        transaction.Commit()
                        Return True
                    Catch ex As Exception
                        transaction.Rollback()
                        Return False
                    End Try
                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function
End Class

Public Class DisplayOrderItem
    Public Property Id As Integer
    Public Property DisplayOrder As Integer
End Class