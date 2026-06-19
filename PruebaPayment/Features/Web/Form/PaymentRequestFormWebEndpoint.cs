namespace PruebaPayment.Features.Web.Form;

public static class PaymentRequestFormWebEndpoint
{
    public static async Task<IResult> PaymentRequestForm()
    {
        var html = await File.ReadAllTextAsync("wwwroot/PaymentRequestForm/index.html");
        return Results.Content(html, "text/html");
    }
}

