import http from 'k6/http';
import { check, sleep} from 'k6';

export const options = {
    stages: [
        { duration: '5s', target: 1000 },
        { duration: '10s', target: 1000 },
        { duration: '20s', target: 0 },
    ],

    thresholds: {
        http_req_duration: ['p(95)<50'],
    },
};


const BASE_URL = `http://localhost:5148`;

export default () => {

    const responses = http.batch([
        ['GET', `${BASE_URL}/courses`, null],
        ['GET', `${BASE_URL}/courses`, null],
        ['GET', `${BASE_URL}/courses`, null],
        ['GET', `${BASE_URL}/courses`, null],
    ])

    responses.forEach(x => {
        const courses = x.json()
        check(courses, { 'retrieved courses': (obj) => courses.length === 3 })
    })


    sleep(1)
}
