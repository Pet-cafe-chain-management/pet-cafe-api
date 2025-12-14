using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using PetCafe.Application.Models.EmailModels;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Application.Services;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string toName, string subject, string body);
    Task SendPaymentSuccessEmailAsync(PaymentSuccessEmailModel model);
}

public class EmailService(AppSettings _appSettings) : IEmailService
{
    public async Task SendEmailAsync(string toEmail, string toName, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_appSettings.SmtpSettings.FromName, _appSettings.SmtpSettings.FromEmail));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = body
        };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_appSettings.SmtpSettings.Host, _appSettings.SmtpSettings.Port, 
                _appSettings.SmtpSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
            await client.AuthenticateAsync(_appSettings.SmtpSettings.Username, _appSettings.SmtpSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            // Log error but don't throw to prevent breaking the payment flow
            Console.WriteLine($"Error sending email: {ex.Message}");
            throw;
        }
    }

    public async Task SendPaymentSuccessEmailAsync(PaymentSuccessEmailModel model)
    {
        var emailBody = GeneratePaymentSuccessEmailTemplate(model);
        await SendEmailAsync(
            toEmail: model.CustomerEmail,
            toName: model.CustomerName,
            subject: "Thanh toán thành công - Pet Cafe",
            body: emailBody
        );
    }

    private string GeneratePaymentSuccessEmailTemplate(PaymentSuccessEmailModel model)
    {
        var bookingTimesHtml = "";
        if (model.BookingTimes != null && model.BookingTimes.Count > 0)
        {
            bookingTimesHtml = "<div style='margin-top: 20px;'>";
            bookingTimesHtml += "<h3 style='color: #2c3e50; margin-bottom: 10px;'>Thông tin đặt chỗ:</h3>";
            bookingTimesHtml += "<ul style='list-style: none; padding: 0;'>";
            foreach (var booking in model.BookingTimes)
            {
                bookingTimesHtml += $@"
                <li style='background-color: #f8f9fa; padding: 15px; margin-bottom: 10px; border-radius: 5px; border-left: 4px solid #28a745;'>
                    <strong>Dịch vụ:</strong> {booking.ServiceName}<br/>
                    <strong>Ngày đặt:</strong> {booking.BookingDate:dd/MM/yyyy}<br/>
                    <strong>Giờ:</strong> {booking.StartTime:HH:mm} - {booking.EndTime:HH:mm}
                </li>";
            }
            bookingTimesHtml += "</ul></div>";
        }

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f4f4f4;
        }}
        .container {{
            background-color: #ffffff;
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }}
        .header {{
            text-align: center;
            padding-bottom: 20px;
            border-bottom: 3px solid #28a745;
            margin-bottom: 30px;
        }}
        .success-icon {{
            font-size: 48px;
            color: #28a745;
            margin-bottom: 10px;
        }}
        .order-info {{
            background-color: #f8f9fa;
            padding: 20px;
            border-radius: 5px;
            margin: 20px 0;
        }}
        .info-row {{
            display: flex;
            justify-content: space-between;
            padding: 8px 0;
            border-bottom: 1px solid #e0e0e0;
        }}
        .info-row:last-child {{
            border-bottom: none;
        }}
        .info-label {{
            font-weight: bold;
            color: #555;
        }}
        .info-value {{
            color: #2c3e50;
        }}
        .footer {{
            margin-top: 30px;
            padding-top: 20px;
            border-top: 1px solid #e0e0e0;
            text-align: center;
            color: #777;
            font-size: 14px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='success-icon'>✓</div>
            <h1 style='color: #28a745; margin: 0;'>Thanh toán thành công!</h1>
        </div>
        
        <p>Xin chào <strong>{model.CustomerName}</strong>,</p>
        
        <p>Cảm ơn bạn đã sử dụng dịch vụ của Pet Cafe. Đơn hàng của bạn đã được thanh toán thành công.</p>
        
        <div class='order-info'>
            <h2 style='color: #2c3e50; margin-top: 0;'>Thông tin đơn hàng</h2>
            <div class='info-row'>
                <span class='info-label'>Mã đơn hàng:</span>
                <span class='info-value'><strong>#{model.OrderNumber}</strong></span>
            </div>
            <div class='info-row'>
                <span class='info-label'>Tên khách hàng:</span>
                <span class='info-value'>{model.CustomerName}</span>
            </div>
            <div class='info-row'>
                <span class='info-label'>Số điện thoại:</span>
                <span class='info-value'>{model.CustomerPhone ?? "N/A"}</span>
            </div>
            <div class='info-row'>
                <span class='info-label'>Email:</span>
                <span class='info-value'>{model.CustomerEmail}</span>
            </div>
            <div class='info-row'>
                <span class='info-label'>Tổng tiền:</span>
                <span class='info-value' style='color: #28a745; font-size: 18px; font-weight: bold;'>{model.TotalAmount:N0} VNĐ</span>
            </div>
            <div class='info-row'>
                <span class='info-label'>Ngày đặt hàng:</span>
                <span class='info-value'>{model.OrderDate:dd/MM/yyyy HH:mm}</span>
            </div>
        </div>

        {bookingTimesHtml}

        <div style='margin-top: 30px; padding: 15px; background-color: #e7f3ff; border-radius: 5px; border-left: 4px solid #2196F3;'>
            <p style='margin: 0;'><strong>Lưu ý:</strong> Vui lòng đến đúng giờ đã đặt. Nếu có thay đổi, vui lòng liên hệ với chúng tôi trước ít nhất 24 giờ.</p>
        </div>

        <div class='footer'>
            <p>Trân trọng,<br/><strong>Pet Cafe Team</strong></p>
            <p style='font-size: 12px;'>Email này được gửi tự động, vui lòng không trả lời email này.</p>
        </div>
    </div>
</body>
</html>";
    }
}


