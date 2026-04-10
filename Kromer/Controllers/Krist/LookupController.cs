using Humanizer;
using Kromer.Models.Api.Krist.Lookup;
using Kromer.Models.Api.Krist.Name;
using Kromer.Models.Api.Krist.Transaction;
using Kromer.Models.Exceptions;
using Kromer.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kromer.Controllers.Krist;

[Route("api/krist/lookup")]
[ApiController]
public class LookupController(LookupService lookupService) : ControllerBase
{
    /// <summary>
    /// Retrieves details about a set of addresses.
    /// </summary>
    /// <param name="addresses">A comma-separated list of addresses to lookup.</param>
    /// <param name="fetchNames">Determines whether to include additional name data for the addresses.</param>
    /// <param name="includePlayers">Indicates whether to include player-related information for the addresses.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="KristLookupAddresses"/> object with the lookup details for the provided addresses.</returns>
    /// <exception cref="KristParameterException">Thrown when the provided address list is empty or invalid.</exception>
    [HttpGet("addresses/{addresses}")]
    public async Task<ActionResult<KristLookupAddresses>> GetAddresses(string addresses,
        [FromQuery] bool fetchNames = false, [FromQuery] bool includePlayers = false)
    {
        var addressList = addresses.Split(',');
        if (addressList.Length == 0)
        {
            throw new KristParameterException("addresses");
        }

        return await lookupService.GetAddresses(addressList.ToList(), fetchNames, includePlayers);
    }


    /// <summary>
    /// Retrieves transaction details for specified addresses with configurable ordering and filtering options.
    /// </summary>
    /// <param name="addresses">
    /// A comma-separated list of addresses to filter the transactions or null to retrieve transactions globally.
    /// </param>
    /// <param name="orderBy">
    /// Specifies the field by which the transactions should be ordered. Valid options are: id, from, to, value, time, sentName, sentMetaname.
    /// </param>
    /// <param name="order">
    /// Specifies the order direction for the transactions. Use "ASC" for ascending or "DESC" for descending.
    /// </param>
    /// <param name="includeMined">
    /// Indicates whether transactions related to mining should be included.
    /// </param>
    /// <param name="limit">
    /// The maximum number of transactions to retrieve. Must be between 1 and 1000.
    /// </param>
    /// <param name="offset">
    /// The number of transactions to skip before starting to include them in the result.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="KristResultTransactions"/> object
    /// with the transaction details based on the provided parameters.
    /// </returns>
    /// <exception cref="KristParameterException">
    /// Thrown when the provided values for "orderBy" or "order" parameters are invalid.
    /// </exception>
    [HttpGet("transactions/{addresses?}")]
    [HttpGet("transactions")]
    public async Task<ActionResult<KristResultTransactions>> GetTransactions(string? addresses,
        [FromQuery] string orderBy = "id",
        [FromQuery] string order = "ASC",
        [FromQuery] bool includeMined = false,
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0)
    {
        var addressList = addresses?.Split(',');

        limit = Math.Clamp(limit, 1, 1000);

        if (!Enum.TryParse<TransactionOrderByParameter>(orderBy.Pascalize(), out var orderByParameter))
        {
            throw new KristParameterException("orderBy");
        }

        if (!Enum.TryParse<OrderParameter>(order.ToLowerInvariant().Pascalize(), out var orderParameter))
        {
            throw new KristParameterException("order");
        }

        return await lookupService.GetTransactions(addressList?.ToList() ?? [], orderByParameter, orderParameter, includeMined, limit, offset);
    }

    /// <summary>
    /// Lookup names from addresses.
    /// </summary>
    /// <param name="addresses"></param>
    /// <param name="orderBy"></param>
    /// <param name="order"></param>
    /// <param name="limit"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    /// <exception cref="KristParameterException"></exception>
    [HttpGet("names/{addresses?}")]
    [HttpGet("names")]
    public async Task<ActionResult<KristResultNames>> GetNames(string? addresses,
        [FromQuery] string orderBy = "name",
        [FromQuery] string order = "ASC",
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0)
    {
        var addressList = addresses?.Split(',');

        limit = Math.Clamp(limit, 1, 1000);

        if (!Enum.TryParse<NameOrderByParameter>(orderBy.Pascalize(), out var orderByParameter))
        {
            throw new KristParameterException("orderBy");
        }

        if (!Enum.TryParse<OrderParameter>(order.ToLowerInvariant().Pascalize(), out var orderParameter))
        {
            throw new KristParameterException("order");
        }

        return await lookupService.GetNames(addressList?.ToList() ?? [], orderByParameter, orderParameter, limit, offset);
    }

    
    /// <summary>
    /// Lookup a name's history.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="orderBy"></param>
    /// <param name="order"></param>
    /// <param name="limit"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    /// <exception cref="KristParameterException"></exception>
    [HttpGet("names/{name}/history")]
    public async Task<ActionResult<KristResultTransactions>> GetNameHistory(string name,
        [FromQuery] string orderBy = "id",
        [FromQuery] string order = "ASC",
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0)
    {
        limit = Math.Clamp(limit, 1, 1000);

        if (!Enum.TryParse<TransactionOrderByParameter>(orderBy.Pascalize(), out var orderByParameter))
        {
            throw new KristParameterException("orderBy");
        }

        if (!Enum.TryParse<OrderParameter>(order.ToLowerInvariant().Pascalize(), out var orderParameter))
        {
            throw new KristParameterException("order");
        }

        return await lookupService.GetNameHistory(name, orderByParameter, orderParameter, limit, offset);
    }

    /// <summary>
    /// Lookup a name's transactions.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="orderBy"></param>
    /// <param name="order"></param>
    /// <param name="limit"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    /// <exception cref="KristParameterException"></exception>
    [HttpGet("names/{name}/transactions")]
    public async Task<ActionResult<KristResultTransactions>> GetNameTransactions(string name,
        [FromQuery] string orderBy = "id",
        [FromQuery] string order = "ASC",
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0)
    {
        limit = Math.Clamp(limit, 1, 1000);

        if (!Enum.TryParse<TransactionOrderByParameter>(orderBy.Pascalize(), out var orderByParameter))
        {
            throw new KristParameterException("orderBy");
        }

        if (!Enum.TryParse<OrderParameter>(order.ToLowerInvariant().Pascalize(), out var orderParameter))
        {
            throw new KristParameterException("order");
        }

        return await lookupService.GetNameTransactions(name, orderByParameter, orderParameter, limit, offset);
    }
}