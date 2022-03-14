using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.PaymentDeposit.Domain.Models;
using Service.PaymentDepositRepository.Domain.Models;
using Service.PaymentProviderBridge.Test3.Models;
using Service.PaymentProviderBridge.Test3.Settings;

namespace Service.PaymentProviderBridge.Test3.Services
{
	public class PaymentProviderGrpcService : IPaymentProviderGrpcService
	{
		private readonly ILogger<PaymentProviderGrpcService> _logger;
		private static readonly HttpClient Client;

		static PaymentProviderGrpcService() => Client = new HttpClient();

		public PaymentProviderGrpcService(ILogger<PaymentProviderGrpcService> logger) => _logger = logger;

		public ValueTask<ProviderDepositGrpcResponse> DepositAsync(ProviderDepositGrpcRequest request)
		{
			SettingsModel settings = Program.Settings;
			string externalUrl = SetTransactionId(settings.ServiceUrl, request.TransactionId);

			DepositRegisterResponse response = null;
			var state = TransactionState.Error;
			int counter = -1;

			while (counter < settings.TryCount)
			{
				counter++;

				response = GetResponse(externalUrl);
				state = GetState(response?.State);

				_logger.LogDebug("Accepted response {@registerResponse} by url {url} for request {@request} (on {tr} try)!", response, externalUrl, request, counter);

				if (state == TransactionState.Approved)
					break;

				Task.Delay(2000);
			}

			return ValueTask.FromResult(new ProviderDepositGrpcResponse
			{
				State = state,
				ExternalId = response?.ExternalId,
				RedirectUrl = state == TransactionState.Approved ? settings.OkUrl : settings.FailUrl
			});
		}

		private static DepositRegisterResponse GetResponse(string externalUrl)
		{
			DepositRegisterResponse registerResponse;

			using (HttpResponseMessage response = Client.GetAsync(externalUrl).Result)
				using (HttpContent content = response.Content)
					registerResponse = JsonConvert.DeserializeObject<DepositRegisterResponse>(content.ReadAsStringAsync().Result);

			return registerResponse;
		}

		private static TransactionState GetState(string state) =>
			state switch {
				"accept" => TransactionState.Accepted,
				"reject" => TransactionState.Rejected,
				"approve" => TransactionState.Approved, 
				_ => TransactionState.Error};

		private static string SetTransactionId(string urlTemplate, Guid? id) => urlTemplate.Replace("#transaction-id#", id.ToString(), StringComparison.OrdinalIgnoreCase);
	}
}