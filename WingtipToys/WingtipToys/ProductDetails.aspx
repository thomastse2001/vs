<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProductDetails.aspx.cs" Inherits="WingtipToys.ProductDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:FormView ID="productDetail" runat="server" ItemType="WingtipToys.Models.Product" SelectMethod="GetProduct" RenderOuterTable="false">
        <ItemTemplate>
            <!--
            <div>
                <h1><%#:Item.ProductName %></h1>
            </div>
            <br />
            <table>
                <tr>
                    <td><img src="/Catalog/Images/<%#:Item.ImagePath %>" style="border:solid; height:300px" alt="<%#:Item.ProductName %>" /></td>
                    <td>&nbsp;</td>
                    <td style="vertical-align:top; text-align:left;">
                        <b>Description:</b><br /><%#:Item.Description %>
                        <br />
                        <span><b>Price:</b>&nbsp;<%#: String.Format("{0:c}", Item.UnitPrice) %></span>
                        <br />
                        <span><b>Product Number:</b>&nbsp;<%#:Item.ProductID %></span>
                        <br />
                        <a href="/AddToCart.aspx?productID=<%#:Item.ProductID %>"><span><b>Add To Cart</b></span></a>
                    </td>
                </tr>
            </table>
            -->
            <div>
                <h1><%#:Item.ProductName %></h1>
                <hr />
                <a href="/AddToCart.aspx?productID=<%#:Item.ProductID %>"><span><b>Add To Cart</b></span></a>
                <dl class="dl-horizontal">
                    <dt>Description</dt>
                    <dd><%#:Item.Description %></dd>
                    <dt>Price</dt>
                    <dd><%#: String.Format("{0:c}", Item.UnitPrice) %></dd>
                    <dt>Product Number</dt>
                    <dd><%#:Item.ProductID %></dd>
                </dl>
                <img src="/Catalog/Images/<%#:Item.ImagePath %>" style="border:solid; height:300px" alt="<%#:Item.ProductName %>" />
            </div>
            <p><a href="/AddToCart.aspx?productID=<%#:Item.ProductID %>"><span><b>Add To Cart</b></span></a></p>
        </ItemTemplate>
    </asp:FormView>
</asp:Content>
