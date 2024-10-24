// import necessary modules
import { check } from 'k6';
import http from 'k6/http';

// define configuration
export const options = {
  // define thresholds
  thresholds: {
    http_req_failed: ['rate<0.01'], // http errors should be less than 1%
    http_req_duration: ['p(99)<1000'], // 99% of requests should be below 1s
  },
};

export default function () {
  // define URL and request body
  const url = 'http://localhost:7080/api/ReceiveBid';
  const payload = JSON.stringify({
    id: "A0ACC75C-7CBE-4F7C-975D-BCE6947E2074",
    state: 4,
    bidAmount: Math.random() * 100,
    userGuid: "FD2204E7-757A-43C8-A220-56B30B33823F",
    lotId: "902b579d-6551-482f-bb1e-4463762cdd80"
});
  const params = {
    headers: {
      'Content-Type': 'application/json',
    },
  };

  // send a post request and save response as a variable
  const res = http.post(url, payload, params);

  // check that response is 200
  check(res, {
    'response code was 200': (res) => res.status == 200,
  });
}