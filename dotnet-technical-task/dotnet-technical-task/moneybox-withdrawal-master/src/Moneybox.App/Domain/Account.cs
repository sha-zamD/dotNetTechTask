using System;

namespace Moneybox.App
{
    public class Account
    {
        public const decimal PayInLimit = 4000m; // Limit for deposits into an account

        public const decimal LowFundsLimit = 500m; // Limit for low funds warning
        
        public const decimal WithdrawalLimit = 100m; // Withdrawal limit per transaction

        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; set; }

        public decimal Withdrawn { get; set; }

        public decimal PaidIn { get; set; }

        public Account(Guid Id, User user, decimal balance, decimal withdrawn, decimal paidIn)
        {
            this.Id = Id;
            this.User = user;
            this.Balance = balance;
            this.Withdrawn = withdrawn;
            this.PaidIn = paidIn;
        }

        public void Transfer(Account to, decimal amount, INotificationService notificationService)
        {
            if (amount <= 0)
            {
                throw new InvalidOperationException("Amount to transfer must be greater than zero");
            }

            if (Balance < amount)
            {
                throw new InvalidOperationException("Insufficient funds to make transfer");
            }

            if (to.PaidIn + amount > PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }

            this.Withdraw(amount, notificationService);  // Withdraw from the source account
            to.Deposit(amount, notificationService);     // Deposit to the target account
        }

        public void Withdraw(decimal amount, INotificationService notificationService)
        {
            if (amount <=0)
            {
                throw new InvalidOperationException("Amount to withdraw must be greater than zero");
            }

            if (WithdrawalLimit < amount)
            {
                throw new InvalidOperationException("Amount to withdraw exceeds withdrawal limit");
            }

            Balance -= amount;
            Withdrawn += amount;

            if (Balance < LowFundsLimit)  // Check if balance is below low funds limit
            {
                notificationService.NotifyFundsLow(User.Email);
            }
        }

        public void Deposit(decimal amount, INotificationService notificationService)
        {
            if (PaidIn + amount > PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }

            if (PayInLimit - (PaidIn + amount) < LowFundsLimit) // Check if the deposit will bring the balance below the low funds limit
            {
                notificationService.NotifyApproachingPayInLimit(User.Email);
            }

            Balance += amount;
            PaidIn += amount;
        }

    }
}