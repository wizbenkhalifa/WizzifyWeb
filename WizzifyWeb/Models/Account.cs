using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace WizzifyWeb.Models
{
    public class Account : IEnumerable<Account>
    {
        [Remote(action: "VerifyUsername", controller: "Accounts")]
        public string username { get; set; }
        [Remote(action: "VerifyPassword", controller: "Accounts")]
        public string password { get; set; }
        [EmailAddress]
        [Remote(action: "VerifyEmail", controller: "Accounts")]
        public string emailAddress { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string ToString => $"{username} | {emailAddress} | {firstName} | {lastName}";

        public Account(string username, string password, string emailAddress, string firstName, string lastName)
        {
            Username(username);
            Password(password);
            EmailAddress(emailAddress);
            this.firstName = firstName;
            this.lastName = lastName;
        }

        public Account() { }

        public Boolean Username(string username) {
            StringValidator strVal = new StringValidator(8, 32, "\"$&");
            try
            {
                // Attempt validation.
                strVal.Validate(username);
                //this.username = username;
                return true;
            }
            catch (ArgumentException exception)
            {
                // Validation failed.
                throw exception;
            }
        }

        public Boolean Password(string password)
        {
            StringValidator strVal = new StringValidator(8, 32, "\"$&");
            try
            {
                // Attempt validation.
                strVal.Validate(password);
                //this.password = password;
                return true;
            }
            catch (ArgumentException exception)
            {
                // Validation failed.
                throw exception;
            }
        }

        public Boolean EmailAddress(string emailAddress)
        {
            if (new EmailAddressAttribute().IsValid(emailAddress)) {
                //this.emailAddress = emailAddress;
                return true;
            }

            return false;
        }

        public IEnumerator<Account> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return obj is Account account &&
                   username == account.username &&
                   password == account.password &&
                   emailAddress == account.emailAddress &&
                   firstName == account.firstName &&
                   lastName == account.lastName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(username, password, emailAddress, firstName, lastName);
        }
    }
}
