using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System;

Updates to keyboard shortcuts … On Thursday, August 1, 2024, Drive keyboard shortcuts will be updated to give you first-letters navigation.Learn more
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BankPortalValidation;

namespace BankPortalApp
{
    public class BankPortal
    {
        private readonly InputValidator _validator;
        private readonly string _directoryPath;
        private readonly string _errorPath;
        private readonly string _registrationLogPath;
        private readonly List<User> _users = new List<User>();
        private readonly List<BankAccount> _accounts = new List<BankAccount>();

        public BankPortal()
        {
            _directoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\", "BankDetails");
            _errorPath = Path.Combine(_directoryPath, "inputError.txt");
            _registrationLogPath = Path.Combine(_directoryPath, "Reg.txt");

            _validator = new InputValidator(_errorPath);
            CheckFilesExist();
        }

        private void CheckFilesExist()
        {
            try
            {

                if (!Directory.Exists(_directoryPath))
                {
                    Directory.CreateDirectory(_directoryPath);

                }

                if (!File.Exists(_errorPath))
                {
                    File.Create(_errorPath).Close();

                }


                if (!File.Exists(_registrationLogPath))
                {
                    File.Create(_registrationLogPath).Close();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating directory or files: {ex.Message}");
            }
        }

        public void RegisterUser()
        {
            string firstName = "";
            string lastName = "";
            string email = "";
            string phoneNo = "";
            string username = "";
            string password = "";

        reFirst:
            Console.Write("First Name: ");
            firstName = Console.ReadLine();
            var firstNameValidation = _validator.ValidateInput(firstName, "firstname");
            if (!firstNameValidation.IsValid)
            {
                Console.WriteLine(firstNameValidation.Message);
                _validator.LogError(firstNameValidation.Message);
                goto reFirst;

            }
        reLast:
            Console.Write("Last Name: ");
            lastName = Console.ReadLine();
            var lastNameValidation = _validator.ValidateInput(lastName, "lastname");
            if (!lastNameValidation.IsValid)
            {
                Console.WriteLine(lastNameValidation.Message);
                _validator.LogError(lastNameValidation.Message);
                goto reLast;

            }
        reEmail:
            Console.Write("Email: ");
            email = Console.ReadLine();
            var emailValidation = _validator.ValidateInput(email, "email");
            if (!emailValidation.IsValid)
            {
                Console.WriteLine(emailValidation.Message);
                _validator.LogError(emailValidation.Message);
                goto reEmail;

            }
        rePhone:
            Console.Write("Phone No: ");
            phoneNo = Console.ReadLine();
            var phoneNoValidation = _validator.ValidateInput(phoneNo, "mobile");
            if (!phoneNoValidation.IsValid)
            {
                Console.WriteLine(phoneNoValidation.Message);
                _validator.LogError(phoneNoValidation.Message);
                goto rePhone;

            }
        reUernameenter:
            Console.Write("Create Username: ");
            username = Console.ReadLine();
            var usernameValidation = _validator.ValidateInput(username, "username");
            if (!usernameValidation.IsValid)
            {
                Console.WriteLine(usernameValidation.Message);
                _validator.LogError(usernameValidation.Message);
                goto reUernameenter;

            }

            if (IsUsernameTaken(username))
            {
                Console.WriteLine("Username already exists. Please enter another username.");
                _validator.LogError("Username already exists.");


            }
        rePassword:
            Console.Write("Create Password: ");
            password = Console.ReadLine();
            var passwordValidation = _validator.ValidateInput(password, "password");
            if (!passwordValidation.IsValid)
            {
                Console.WriteLine(passwordValidation.Message);
                _validator.LogError(passwordValidation.Message);
                goto rePassword;

            }




            User newUser = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNo = phoneNo,
                Username = username,
                Password = password
            };

            _users.Add(newUser);
            Console.WriteLine("User Registered Successfully...");


            LogRegistration(newUser);
        }

        private bool IsUsernameTaken(string username)
        {
            foreach (var user in _users)
            {
                if (user.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public void CreateAccount()
        {
            long accountNumber = 0;
            string ifscCode = "";
            decimal balance = 0;

        reAccount:
            Console.Write("Enter Account No: ");
            string accountNumberInput = Console.ReadLine();
            var accountNumberValidation = _validator.ValidateInput(accountNumberInput, "id");
            if (!accountNumberValidation.IsValid)
            {
                Console.WriteLine(accountNumberValidation.Message);
                _validator.LogError(accountNumberValidation.Message);
                goto reAccount;
            }
            if (!long.TryParse(accountNumberInput, out accountNumber))
            {
                Console.WriteLine("Invalid account number format. Please enter a valid number.");
                _validator.LogError("Invalid account number format.");

            }
        reIfsc:
            Console.Write("Your IFSC Code: ");
            ifscCode = Console.ReadLine();
            var ifscCodeValidation = _validator.ValidateInput(ifscCode, "ifsc");
            if (!ifscCodeValidation.IsValid)
            {
                Console.WriteLine(ifscCodeValidation.Message);
                _validator.LogError(ifscCodeValidation.Message);
                goto reIfsc;
            }
        reBalance:
            Console.Write("Add Account Balance: ");
            string balanceInput = Console.ReadLine();
            var balanceValidation = _validator.ValidateInput(balanceInput, "balance");
            if (!balanceValidation.IsValid)
            {
                Console.WriteLine(balanceValidation.Message);
                _validator.LogError(balanceValidation.Message);
                goto reBalance;

            }

            if (!decimal.TryParse(balanceInput, out balance))
            {
                Console.WriteLine("Invalid balance format. Please enter a valid decimal number.");
                _validator.LogError("Invalid balance format.");

            }


            BankAccount newAccount = new BankAccount(accountNumber, ifscCode, balance);

            _accounts.Add(newAccount);
            Console.WriteLine("Account added successfully...");


        }


        public void ViewAccounts()
        {
            Console.WriteLine($"Total Added Accounts List is {_accounts.Count}");
            foreach (var account in _accounts)
            {
                Console.WriteLine($"Account No: {account.GetAccountNumber()}");
                Console.WriteLine($"IFSC Code: {account.GetIFSC()}");
                Console.WriteLine($"Balance: {account.GetBalance()}");
                Console.WriteLine();
            }
        }

        public void ViewAllUsers()
        {
            Console.WriteLine($"Total Users Added -> {_users.Count}");
            foreach (var user in _users)
            {
                Console.WriteLine($"Username: {user.Username}");
                Console.WriteLine($"Email: {user.Email}");
                Console.WriteLine($"Phone No: {user.PhoneNo}");
                Console.WriteLine();
            }
        }

        private void LogRegistration(User user)
        {
            try
            {
                using (StreamWriter writer = File.AppendText(_registrationLogPath))
                {
                    writer.WriteLine($"User Registered - {DateTime.Now}");
                    writer.WriteLine($"Name: {user.FirstName} {user.LastName}");
                    writer.WriteLine($"Email: {user.Email}");
                    writer.WriteLine($"Phone No: {user.PhoneNo}");
                    writer.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging registration: {ex.Message}");
            }
        }
    }

    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class BankAccount
    {
        private long _accountNumber;
        private string _ifsc;
        private decimal _balance;

        public BankAccount(long accountNumber, string ifsc, decimal balance)
        {
            _accountNumber = accountNumber;
            _ifsc = ifsc;
            _balance = balance;
        }

        public long GetAccountNumber()
        {
            return _accountNumber;
        }

        public string GetIFSC()
        {
            return _ifsc;
        }

        public decimal GetBalance()
        {
            return _balance;
        }

        public void Deposit(decimal amount)
        {
            if (amount > 0)
            {
                _balance += amount;
                Console.WriteLine($"Deposited {amount} successfully. New balance: {_balance}");
            }
            else
            {
                Console.WriteLine("Invalid deposit amount.");
            }
        }

        public void Withdraw(decimal amount)
        {
            if (amount > 0 && amount <= _balance)
            {
                _balance -= amount;
                Console.WriteLine($"Withdrawn {amount} successfully. New balance: {_balance}");
            }
            else
            {
                Console.WriteLine("Invalid withdrawal amount or insufficient balance.");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            BankPortal portal = new BankPortal();

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Welcome to our Bank Portal");
                Console.WriteLine("Press 1: Registration");
                Console.WriteLine("Press 2: Create Account");
                Console.WriteLine("Press 3: View Accounts");
                Console.WriteLine("Press 4: All Users List");
                Console.WriteLine("Press 5: Exit");
                Console.Write("Enter Your Choice: ");
                int choice;
                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        portal.RegisterUser();
                        break;

                    case 2:
                        portal.CreateAccount();
                        break;

                    case 3:
                        portal.ViewAccounts();
                        break;

                    case 4:
                        portal.ViewAllUsers();
                        break;

                    case 5:
                        exit = true;
                        Console.WriteLine("Thanks for visiting our portal.");
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}