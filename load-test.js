import http from 'k6/http';
import { check, sleep} from 'k6';

export const options = {
    stages: [
        { duration: '10s', target: 300 },
        { duration: '20s', target: 300 },
        { duration: '10s', target: 0 },
    ],

    thresholds: {
        http_req_duration: ['p(99)<20'],
    },
};

const BASE_URL = `http://localhost:5148`;

export default () => {
    const courses = http.get(`${BASE_URL}/courses`).json()
    check(courses, { 'retrieved courses': (obj) => courses.length === 2 })
    sleep(1)
}
