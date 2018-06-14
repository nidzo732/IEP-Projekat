var myTrans = {};
$(document).ready(() =>
{
    var hub = $.connection.auctionHub;
    hub.client.PriceUpdate = function (id, currentAmmount, lastBidder, prevBidder, tokenAmmount)
    {
        var elem = $("#auction-item-" + id);
        if (elem.length == 0) return;
        if (window.location.href.indexOf("details") != -1 || window.location.href.indexOf("Details") != -1)
        {
            refreshBidList(id);
        }
        var price = elem.find(".auction-list-item-current-price");
        var bidder = elem.find(".auction-list-item-last-bidder");
        var oldPrice = parseFloat($(price).text());
        var oldBidder = bidder.text();
        var tokens = parseFloat($("#my-tokens").text());
        price.text(currentAmmount.toFixed(2));
        bidder.text(lastBidder);
        var tokenPrice = currentAmmount / tokenAmmount;
        if (lastBidder == myName)
        {
            tokens -= tokenAmmount;
        }
        if (prevBidder == myId)
        {
            tokens += oldPrice / tokenPrice;
        }
        $("#my-tokens").text(tokens.toFixed(2));
    };
    $.connection.hub.start().done(function ()
    {
    });
});
async function asyncAjax(url, data = {}, method = 'POST')
{
    if (data instanceof FormData)
    {
        var promise = new Promise((accept, reject) =>
        {
            $.ajax(url, {
                method: method,
                data: data,
                processData: false,
                contentType: false,
                success: function (result)
                {
                    accept(result);
                },
                error: (xhr, error, thrown) =>
                {
                    err = "";
                    if (error) err = error;
                    if (thrown) err += " " + thrown;
                    $("*").unblock();
                    showAlertBox("Došlo je do greške u mrežnoj komunikaciji: " + err);
                    reject(err);
                }
            });
        });
        return await promise;
    }
    else
    {
        var promise = new Promise((accept, reject) =>
        {
            $.ajax(url, {
                method: method,
                data: data,
                success: function (result)
                {
                    accept(result);
                },
                error: (xhr, error, thrown) =>
                {
                    err = "";
                    if (error) err = error;
                    if (thrown) err += " " + thrown;
                    $("*").unblock();
                    showAlertBox("Došlo je do greške u mrežnoj komunikaciji: " + err);
                    reject(err);
                }
            });
        });
        return await promise;
    }
}
async function postBid(id, amount, button)
{
    
    var increment = parseFloat(amount);
    var expVal = parseFloat($(button).siblings(".auction-list-item-current-price").text());
    var result = { status: "OK" };
    if (amount <= 0)
    {
        result = { status: "ERR:" + "Increment must be a positive non-zero number" };
    }
    if (result.status == "OK")
    {
        try
        {
            result = await asyncAjax("/Auctions/Bid",
                {
                    id: id,
                    increment: increment,
                    expVal: expVal
                }, "POST");
        }
        catch(ex)
        {
            result = { status: "Network error" };
        }
    }
    $(button).siblings(".auction-list-item-current-price").text(parseFloat(result.ammount).toFixed(2));
    $(button).siblings(".auction-list-item-last-bidder").text(result.lastBidder);
    $("#my-tokens").text(parseFloat(result.tokens).toFixed(2));
    if (result.status != "OK")
    {
        $(button).attr("title", result.status.substr(4));
        $(button).tooltip("enable");
        $(button).tooltip("show");
        setTimeout(() => { $(button).tooltip("hide"); $(button).tooltip("disable"); }, 1000);
    }
    else
    {
        myTrans[id] = true;
    }
}
async function refreshBidList(id)
{
    $("#bidlist").html(await asyncAjax("/Auctions/BidsList", { id: id }, "GET"));
}