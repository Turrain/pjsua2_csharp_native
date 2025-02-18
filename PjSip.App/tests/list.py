import unittest
import requests
import json
import time

class TestSipCallController(unittest.TestCase):

    BASE_URL = "http://localhost:5000/api"  # Replace with your API's base URL
        
    def test_get_all_accounts(self):
        """Test retrieving all registered SIP accounts."""
        # First, register a test account to ensure there's data
      #  self.test_register_account_success()

        endpoint = f"{self.BASE_URL}/SipAccounts"
        response = requests.get(endpoint)

        self.assertEqual(response.status_code, 200)
        data = response.json()
        
        # Verify response is a list
        self.assertIsInstance(data, list)
        
        # Verify at least one account exists
        self.assertGreater(len(data), 0)
        
        # Verify account structure
        account = data[0]
        expected_fields = [
            "id", 
            "accountId", 
            "username", 
            "domain", 
            "registrarUri", 
            "isActive", 
            "createdAt", 
        ]
        for field in expected_fields:
            self.assertIn(field, account)
        print(data)
        # Verify the account we just created is in the list
        found_account = next(
            (acc for acc in data if acc["username"] == '1000'), 
            None
        )
        self.assertIsNotNone(found_account)
        self.assertEqual(found_account["username"], "1000")
        self.assertEqual(found_account["domain"], "localhost")

 
if __name__ == '__main__':
    unittest.main()