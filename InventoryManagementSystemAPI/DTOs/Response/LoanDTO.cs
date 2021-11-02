using InventoryManagementSystemAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.DTOs
{
    public class UserLoanResponseDTO
    {
        public List<BasicUserResponseDTO> Users { get; set; }

        public UserLoanItemResponseDTO LoanItem { get; set; }
    }

    public class UserLoansResponseDTO
    {
        public UserResponseDTO User { get; set; }

        public List<UserLoanItemResponseDTO> LoanItems { get; set; }
    }

    public class UserLoanHistoryResponseDTO
    {
        public BasicUserResponseDTO User { get; set; }

        public List<UserLoanItemResponseDTO> LoanItems { get; set; }
    }
    public class GetAllUserLoansResponseDTO
    {
        public BasicUserResponseDTO User { get; set; }

        public BasicItemResponseDTO LoanItem { get; set; }

        public bool IsReturned { get; set; }

        public string LoanDate { get; set; }
        public string ReturnDate { get; set; }
    }
}
