import unittest
import requests
import json

class TestSipCallController(unittest.TestCase):

    BASE_URL = "http://localhost:5000/api"  # Replace with your API's base URL

    def test_register_account_success(self):
        """Test successful account registration."""
        endpoint = f"{self.BASE_URL}/SipAccounts"
        payload = {
            "username": "1000",
            "password": "1000",
            "domain": "localhost",
            "registrarUri": "sip:localhost"
        }
        headers = {'Content-Type': 'application/json'}
        response = requests.post(endpoint, data=json.dumps(payload), headers=headers)

        self.assertEqual(response.status_code, 200)
        data = response.json()
        self.assertIn("accountId", data)
        self.assertEqual(data["username"], "1000")

        # Store the account ID for later use
        self.account_id = data["accountId"]

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