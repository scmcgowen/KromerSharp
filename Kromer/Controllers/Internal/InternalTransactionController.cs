using Kromer.Attributes;
using Kromer.Services;
using Kromer.Models.Api.Internal;
using Kromer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Kromer.Models.Api.Krist.Transaction;

namespace Kromer.Controllers.Internal;

[Route("api/_internal/transactions")]
[ApiController]
[RequireInternalKey]
public class InternalTransactionController(TransactionRepository transactionRepository, DiscordService discordService): ControllerBase
{
    [HttpPost("force-transfer")]
    public async Task<ActionResult<KristResultTransaction>> ForceTransfer(ForceTransferRequest request)
    {

        var transaction = await transactionRepository.ForceCreateTransactionAsync(request.From, request.To, request.Amount, request.MetaData);

        var ipAddress = HttpContext.Connection.RemoteIpAddress;
        if (ipAddress is not null)
        {
            await discordService.SendForceTransferAlertAsync(request.From, request.To, request.Amount, ipAddress);
        }

        return new KristResultTransaction
        {
            Ok = true,
            Transaction = transaction,
        };
    }
}
