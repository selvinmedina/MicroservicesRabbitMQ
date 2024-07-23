using MicroRabbit.Banking.Application.Models;
using MicroRabbit.Banking.Domain.Models;

namespace MicroRabbit.Banking.Application.Interfaces;

public interface IAccountService
{
    public IEnumerable<Account> GetAccounts();

    public void Transfer(AccountTransfer accountTransfer);

}
