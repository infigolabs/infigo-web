namespace UMServer.Services
{
	public interface IEmailService
	{
		void Send(string toAddress, string subject, string body);
	}
}