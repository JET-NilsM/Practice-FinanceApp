using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TransactionService.Models;

namespace TransactionService.Controllers;

public class TransactionController : ControllerBase
{
    private TransactionsModel _transactionsModel;

    public TransactionController()
    {
        _transactionsModel = new TransactionsModel();
    }

    //Create transaction (Create)
    [HttpPost] //This attribute is needed to tell the program that this method should only handle Post requests (so creating data)
    //Returns an IActionResult to allow multiple return types based on the result of what needed to be done. So if the user isn't properly authenticated, it returns a 
    //a BadRequest. If the action is successfull then Ok() is returned.
    //Since I'm currently only working with lists async is not really necessary but when I get to actually using a service instance,
    //that communicates with a repository with a connection to a database, it is necessary so I'm using it as a preparation. 
    public async Task<IActionResult> CreateTransaction(int senderID, int receiverID, float amount, string description)
    // public async Task<IActionResult> CreateTransaction(Transaction transaction)
    {
        /*Account senderAccount = _accountsModel.Accounts.Where((account) => account.Id == senderID).FirstOrDefault();
        if (senderAccount == null)
            return BadRequest("Sender ID is invalid");
        //Considered NotFound as well but I think (?) that is only to indicate that webpages do not exist since it returns a 404 (aka webpage not found)

        Account receiverAccount = _accountsModel.Accounts.Where((account) => account.Id == receiverID).FirstOrDefault();
        if (receiverAccount == null)
            return BadRequest("Receiver ID is invalid");

        bool hasEnoughMoney = senderAccount.AccountBalance >= amount;
        if (!hasEnoughMoney)
            return BadRequest("Sender account does not have enough money");

        Transaction newTransaction = new Transaction
        {
            SenderID = senderID,
            ReceiverID = receiverID,
            Amount = amount,
            Description = description
        };

        _transactionsModel.Transactions.Add(newTransaction);
        */

        //I'm not sure what the best approach of updating the balance of both accounts would be?
        //I could let the transaction class handle it but that should probably only be a data container rather than partly a controller
        //I could also manually update the balances here but that would mean a TransactionController handles account related logic.
        //Rather than only creating a transaction and storing it.
        //So another way could be to create an AccountController but as far as I understood MVC conventions a controller is meant specifically fo

        return Ok();
    }


    //Retrieve existing transaction data (Read). Here only a transactionID is necessary. 
    [HttpGet]
    public async Task<IActionResult> GetTransaction(int transactionID)
    {
        Transaction requestedTransaction = _transactionsModel.Transactions
            .Where((transaction) => transaction.ID == transactionID).FirstOrDefault();
        if (requestedTransaction == null)
            return BadRequest("Transaction not found");

        return Ok(requestedTransaction);
    }

    //Update transaction data not really applicable to transactions? (Update) since it's static data. Unless there's like a mistake in the description that for some reason should be fixed,
    //there's not much to update.

    //Delete transaction data, maybe in the form of reversing a transaction rather than entirely deleting its existence?
    //Since this type of activity should always be retrievable and kept in a history
    //I'll still write both if I have time.
    [HttpDelete]
    public async Task<IActionResult> DeleteTransaction(int transactionID)
    {
        Transaction transactionToDelete = _transactionsModel.Transactions.Where((transaction) => transaction.ID == transactionID).FirstOrDefault();
        if (transactionToDelete == null)
            return BadRequest("Transaction not found");

        _transactionsModel.Transactions.Remove(transactionToDelete);

        return Ok("Transaction removed from collection successfully");
    }

    //An alternative to actually removing a transaction from the database and manually adjusting the accounts balances.
    //I am not sure which httppost method to use for this though. Since it depends on the actual implementation of this Reverse() method. 
    public async Task<IActionResult> ReverseTransaction(int transactionID)
    {
        Transaction transactionToReverse = _transactionsModel.Transactions.Where((transaction) => transaction.ID == transactionID).FirstOrDefault();
        if (transactionToReverse == null)
            return BadRequest("Transaction not found");

        //Copilot generated the following lines, but I think it would be better to use the existing "create transaction" flow, but swapping the sender and receiver IDs.  
        /*//Reversing a transaction would mean that the sender and receiver accounts are swapped and the amount is negated.
        int temp = transactionToReverse.SenderID;
        transactionToReverse.SenderID = transactionToReverse.ReceiverID;
        transactionToReverse.ReceiverID = temp;
        transactionToReverse.Amount = -transactionToReverse.Amount;

        //Add the reversed transaction to the collection*/

        return Ok();
    }
}