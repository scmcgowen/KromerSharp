using Kromer.Attributes;
using Kromer.Models.Api.Internal;
using Kromer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Kromer.Models.Api.Krist.Transaction;

namespace Kromer.Controllers.Internal;

[Route("api/_internal/transactions")]
[ApiController]
[RequireInternalKey]
public class InternalTransactionController(TransactionRepository transactionRepository): ControllerBase
{
    [HttpPost("forceTransfer")]
    public async Task<ActionResult<KristResultTransaction>> forceTransfer(ForceTransferRequest request)
    {

        var transaction = await transactionRepository.ForceCreateTransactionAsync(request.From, request.To, request.Amount, request.MetaData);

        return new KristResultTransaction
        {
            Ok = true,
            Transaction = transaction,
        };
    }
}
