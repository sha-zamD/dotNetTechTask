using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class WithdrawMoney
    {
        private IAccountRepository accountRepository;
        private INotificationService notificationService;

        public WithdrawMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this.accountRepository = accountRepository;
            this.notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, decimal amount)
        {
            // TODO:

            var account = this.accountRepository.GetAccountById(fromAccountId);

            account.Withdraw(amount, this.notificationService);     // Performing the withdrawal

            this.accountRepository.Update(account);   // Updating the account

        }
    }
}
