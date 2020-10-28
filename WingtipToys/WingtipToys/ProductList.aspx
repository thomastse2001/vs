<%@ Page Title="Products" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProductList.aspx.cs" Inherits="WingtipToys.ProductList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section>
        <div>
            <hgroup>
                <h2><%: Page.Title %></h2>
            </hgroup>
            <%--
            <asp:ListView ID="productList1" runat="server" DataKeyNames="ProductID" GroupItemCount="4" ItemType="WingtipToys.Models.Product" SelectMethod="GetProducts">
                <EmptyDataTemplate>
                    <table>
                        <tr>
                            <td>No data was returned.</td>
                        </tr>
                    </table>
                </EmptyDataTemplate>
                <EmptyItemTemplate>
                    <td />
                </EmptyItemTemplate>
                <GroupTemplate>
                    <tr id="itemPlaceholderContainer" runat="server">
                        <td id="itemPlaceholder" runat="server"></td>
                    </tr>
                </GroupTemplate>
                <ItemTemplate>
                    <td runat="server">
                        <table>
                            <tr>
                                <td>
                                    <a href="ProductDetails.aspx?productID=<%#:Item.ProductID%>">
                                        <img src="/Catalog/Images/Thumbs/<%#:Item.ImagePath%>" width="100" height="75" style="border:solid" />
                                    </a>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <a href="ProductDetails.aspx?productID=<%#:Item.ProductID%>"><span><%#:Item.ProductName%></span></a>
                                    <br />
                                    <span><b>Price: </b><%#:String.Format("{0:c}", Item.UnitPrice)%></span>
                                    <br />
                                    <a href="/AddToCart.aspx?productID=<%#:Item.ProductID %>"><span class="ProductListItem"><b>Add To Cart</b></span></a>
                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                            </tr>
                        </table>

                        <div class="col-ld-2">
                            <div class="panel panel-default">
                                <div class="panel-heading"><a href="ProductDetails.aspx?productID=<%#:Item.ProductID%>"><span><%#:Item.ProductName%></span></a></div>
                                <div class="panel-body">
                                    <b>Price: </b><%#:String.Format("{0:c}", Item.UnitPrice)%>
                                    <br />
                                    <a href="/AddToCart.aspx?productID=<%#:Item.ProductID %>"><span class="ProductListItem"><b>Add To Cart</b></span></a>
                                    <br />
                                    <a href="ProductDetails.aspx?productID=<%#:Item.ProductID%>">
                                        <img src="/Catalog/Images/Thumbs/<%#:Item.ImagePath%>" width="100" height="75" style="border:solid" />
                                    </a>
                                </div>
                            </div>
                            <p>&nbsp;</p>
                        </div>

                    </td>
                </ItemTemplate>
                <LayoutTemplate>
                    <div class="container">
                        <div class="row">
                            <table id="groupPlaceholderContainer" runat="server" style="width:100%">
                                <tr id="groupPlaceholder"></tr>
                            </table>
                        </div>
                    </div>
                </LayoutTemplate>

            </asp:ListView>
            --%>

            <asp:ListView ID="productList" runat="server"
                DataKeyNames="ProductID"
                ItemType="WingtipToys.Models.Product" SelectMethod="GetProducts">
                <EmptyDataTemplate>
                    <div>No data was returned.</div>
                </EmptyDataTemplate>
                <EmptyItemTemplate>
                </EmptyItemTemplate>
                <ItemTemplate>
                    <div class="col-sm-2" runat="server">
                            <div class="panel panel-default">
                                <div class="panel-heading"><a href="ProductDetails.aspx?productID=<%#:Item.ProductID%>"><span><%#:Item.ProductName%></span></a></div>
                                <div class="panel-body">
                                    <b>Price: </b><%#:String.Format("{0:c}", Item.UnitPrice)%>
                                    <br />
                                    <a href="/AddToCart.aspx?productID=<%#:Item.ProductID %>"><span class="ProductListItem"><b>Add To Cart</b></span></a>
                                    <br />
                                    <a href="ProductDetails.aspx?productID=<%#:Item.ProductID%>">
                                        <img src="/Catalog/Images/Thumbs/<%#:Item.ImagePath%>" width="100" height="75" style="border:solid" />
                                    </a>
                                </div>
                            </div>
                        </div>
                </ItemTemplate>
                <LayoutTemplate>
                    <div runat="server" id="itemPlaceholderContainer" class="container">
                        <div class="row">
                            <div runat="server" id="itemPlaceholder"></div>
                        </div>
                    </div>
                </LayoutTemplate>
            </asp:ListView>
        </div>
    </section>
</asp:Content>
