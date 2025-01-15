<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.vb" Inherits="ColorsManager._Default" %>

<%@ Import Namespace="System.Data.SqlClient" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <h2 style="text-align: center;">ניהול טבלת צבעים</h2>
            <asp:GridView ID="ColorsGridView" runat="server" AutoGenerateColumns="False" Width="100%" BorderStyle="Solid" BorderWidth="1px"
                DataKeyNames="Id" OnRowEditing="ColorsGridView_RowEditing" 
                OnRowUpdating="ColorsGridView_RowUpdating" OnRowDeleting="ColorsGridView_RowDeleting" 
                OnRowCancelingEdit="ColorsGridView_RowCancelingEdit" CssClass="text-center sortable-grid">
                <HeaderStyle HorizontalAlign="Center" CssClass="grid-header" />
                <RowStyle CssClass="grid-row" />
                <Columns>
                    <asp:TemplateField HeaderText="שם צבע" ItemStyle-HorizontalAlign="Center">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtColorNameEdit" runat="server" Text='<%# Bind("ColorName") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <div class="drag-handle">&#8942;</div>
                            <asp:HiddenField ID="hdnId" runat="server" Value='<%# Eval("Id") %>' />
                            <%# Eval("ColorName") %>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>


                                        <asp:TemplateField HeaderText="מחיר" ItemStyle-HorizontalAlign="Center">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtPriceEdit" runat="server" Text='<%# Bind("Price") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <%# Eval("Price") %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="סדר הצגה" ItemStyle-HorizontalAlign="Center">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtDisplayOrderEdit" runat="server" Text='<%# Bind("DisplayOrder") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <%# Eval("DisplayOrder") %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="קוד צבע" ItemStyle-HorizontalAlign="Center">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtColorCodeEdit" runat="server" Text='<%# Bind("ColorCode") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# Eval("ColorCode") %>' style='<%# "background-color:" & Eval("ColorCode") & "; padding:5px; border:1px solid black; display:block;" %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="במלאי" ItemStyle-HorizontalAlign="Center">
                        <EditItemTemplate>
                            <asp:CheckBox ID="chkInStockEdit" runat="server" Checked='<%# Bind("InStock") %>' />
                        </EditItemTemplate>
                        <ItemTemplate>
                            <%# IIf(Eval("InStock"), "כן", "לא") %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:CommandField ShowEditButton="True" ShowDeleteButton="True" ItemStyle-HorizontalAlign="Center" />
                </Columns>
            </asp:GridView>

            <br />
            <h3>הוספת צבע חדש</h3>
            <table>
                <tr>
                    <td>שם הצבע: *</td>
                    <td><asp:TextBox ID="txtColorName" runat="server" CssClass="form-control"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>מחיר: *</td>
                    <td><asp:TextBox ID="txtPrice" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>סדר הצגה:</td>
                    <td><asp:TextBox ID="txtDisplayOrder" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>בחר צבע:</td>
                    <td><asp:TextBox ID="txtColorCode" runat="server" CssClass="form-control" TextMode="Color"></asp:TextBox></td>
                    <td><asp:Label ID="lblColorPreview" runat="server" Text=" " Width="50px" Height="20px" BorderStyle="Solid"></asp:Label></td>
                </tr>
                <tr>
                    <td>האם במלאי?</td>
                    <td><asp:CheckBox ID="chkInStock" runat="server" /></td>
                </tr>
            </table>
            <br />
            <asp:Button ID="btnAddNewColor" runat="server" Text="הוסף צבע חדש" CssClass="btn btn-success" OnClick="btnAddNewColor_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>

     <style>
        .grid-header {
            background-color: #f8f9fa;
            font-weight: bold;
            text-align: center !important;
            padding: 10px;
        }
        
        .grid-row {
            cursor: move;
        }
        
        .grid-row td {
            text-align: center !important;
        }
        
        .drag-handle {
            cursor: move;
            display: inline-block;
            margin-right: 10px;
            color: #666;
        }
        
        .ui-sortable-helper {
            display: table;
            background-color: #fff;
            box-shadow: 0 2px 5px rgba(0,0,0,0.15);
        }

        .form-control {
            text-align: center;
        }
    </style>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://code.jquery.com/ui/1.13.2/jquery-ui.min.js"></script>
    <link href="https://code.jquery.com/ui/1.13.2/themes/base/jquery-ui.css" rel="stylesheet" />

    <script>
        function initializeSortable() {
            $(".sortable-grid tbody").sortable({
                helper: function (e, tr) {
                    var $originals = tr.children();
                    var $helper = tr.clone();
                    $helper.children().each(function (index) {
                        $(this).width($originals.eq(index).width());
                    });
                    return $helper;
                },
                handle: ".drag-handle",
                update: function (event, ui) {
                    var orderData = [];
                    $(this).find("tr").each(function (index) {
                        var id = $(this).find("[id*=hdnId]").val();
                        orderData.push({
                            Id: parseInt(id),
                            DisplayOrder: index + 1
                        });
                    });

                    $.ajax({
                        type: "POST",
                        url: "Default.aspx/UpdateDisplayOrder",
                        data: JSON.stringify({ orderData: orderData }),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            if (response.d) {
                                __doPostBack('<%=UpdatePanel1.ClientID%>', '');
                            } else {
                                alert('אירעה שגיאה בעדכון סדר התצוגה');
                            }
                        },
                        error: function (error) {
                            alert('אירעה שגיאה בעדכון סדר התצוגה');
                        }
                    });
                }
            }).disableSelection();
        }

        $(document).ready(function () {
            initializeSortable();
        });

        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(function () {
            initializeSortable();
        });
    </script>
</asp:Content>