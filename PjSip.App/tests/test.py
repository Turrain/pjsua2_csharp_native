import unittest
import requests
import json

class TestSipCallController(unittest.TestCase):

    BASE_URL = "http://localhost:5000/api"  # Replace with your API's base URL
    # def test_clear_all_accounts(self):
    #     """Test clearing all registered SIP accounts."""
    #     endpoint = f"{self.BASE_URL}/SipAccounts"
    #     response = requests.delete(endpoint)

    #     self.assertEqual(response.status_code, 200)
    #     data = response.json()
    #     self.assertIn("message", data)
    #     self.assertEqual(data["message"], "All SIP accounts have been deleted")
        
    # def test_register_account_success(self):
    #     """Test successful account registration."""
    #     endpoint = f"{self.BASE_URL}/SipAccounts"
    #     payload = {
    #         "username": "1000",
    #         "password": "1000",
    #         "domain": "localhost",
    #         "registrarUri": "sip:localhost"
    #     }
    #     headers = {'Content-Type': 'application/json'}
    #     response = requests.post(endpoint, data=json.dumps(payload), headers=headers)

    #     self.assertEqual(response.status_code, 200)
    #     data = response.json()
    #     self.assertIn("accountId", data)
    #     self.assertEqual(data["username"], "1000")

    #     # Store the account ID for later use
    #     self.account_id = data["accountId"]

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
            (acc for acc in data if acc["accountId"] == 1000), 
            None
        )
        self.assertIsNotNone(found_account)
        self.assertEqual(found_account["username"], "1000")
        self.assertEqual(found_account["domain"], "localhost")
    # def test_register_account_success2(self):
    #     """Test successful account registration."""
    #     endpoint = f"{self.BASE_URL}/SipAccounts"
    #     payload = {
    #         "username": "1001",
    #         "password": "1001",
    #         "domain": "localhost",
    #         "registrarUri": "sip:localhost"
    #     }
    #     headers = {'Content-Type': 'application/json'}
    #     response = requests.post(endpoint, data=json.dumps(payload), headers=headers)

    #     self.assertEqual(response.status_code, 200)
    #     data = response.json()
    #     self.assertIn("accountId", data)
    #     self.assertEqual(data["username"], "1001")

    #     # Store the account ID for later use
    #     self.account_id = data["accountId"]
    # def test_make_call_success(self):
    #     """Test making a call successfully."""
    #     # First, register an account (assuming registration works)
    #     self.test_register_account_success() # Call the registration test to ensure an account exists

    #     endpoint = f"{self.BASE_URL}/SipCall"
    #     payload = {
    #         "accountId": self.account_id,
    #         "destination": "sip:destination@example.com"
    #     }
    #     headers = {'Content-Type': 'application/json'}
    #     response = requests.post(endpoint, data=json.dumps(payload), headers=headers)

    #     self.assertEqual(response.status_code, 200)
    #     data = response.json()
    #     self.assertIn("callId", data)
    #     self.assertEqual(data["status"], "EARLY")  # Or whatever initial status you expect
    #     self.call_id = data["callId"] # Store call id for later tests

    # def test_make_call_failure_invalid_account(self):
    #     """Test making a call with an invalid account ID."""
    #     endpoint = f"{self.BASE_URL}/SipCall"
    #     payload = {
    #         "accountId": "invalid_account_id",
    #         "destination": "sip:destination@example.com"
    #     }
    #     headers = {'Content-Type': 'application/json'}
    #     response = requests.post(endpoint, data=json.dumps(payload), headers=headers)

    #     self.assertEqual(response.status_code, 500)  # Or 400, depending on your error handling
    #     data = response.json()
    #     self.assertIn("error", data)

    # def test_hangup_call_success(self):
    #      """Test hanging up a call successfully."""
    #      #First make a call
    #      self.test_make_call_success()

    #      endpoint = f"{self.BASE_URL}/SipCall/{self.call_id}/hangup"
    #      response = requests.post(endpoint) # No body needed for hangup

    #      self.assertEqual(response.status_code, 200)
    #      data = response.json()
    #      self.assertIn("message", data)
    #      self.assertEqual(data["message"], "Call terminated successfully")

    # def test_hangup_call_invalid_call_id(self):
    #     """Test hanging up a call with an invalid call ID."""
    #     endpoint = f"{self.BASE_URL}/SipCall/9999/hangup"  # Non-existent call ID
    #     response = requests.post(endpoint)

    #     self.assertEqual(response.status_code, 400)  # Or 500, depending on your error handling
    #     data = response.json()
    #     self.assertIn("error", data)
    #     self.assertEqual(data["error"], "Invalid call ID")

    # def test_get_call_status_success(self):
    #     """Test getting the status of a call successfully."""
    #     # First, make a call (assuming making a call works)
    #     self.test_make_call_success()

    #     endpoint = f"{self.BASE_URL}/SipCall/{self.call_id}/status"
    #     response = requests.get(endpoint)

    #     self.assertEqual(response.status_code, 200)
    #     data = response.json()
    #     self.assertIn("callId", data)
    #     self.assertEqual(data["callId"], self.call_id)
    #     self.assertIn("status", data)

    # def test_get_call_status_invalid_call_id(self):
    #     """Test getting the status of a call with an invalid call ID."""
    #     endpoint = f"{self.BASE_URL}/SipCall/9999/status"
    #     response = requests.get(endpoint)

    #     self.assertEqual(response.status_code, 404)  # Or 400, depending on your error handling
    #     data = response.json()
    #     self.assertIn("error", data)
    #     self.assertEqual(data["error"], "Call not found")


if __name__ == '__main__':
    unittest.main()