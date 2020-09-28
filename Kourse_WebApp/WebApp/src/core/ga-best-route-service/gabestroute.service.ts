import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Route, Place } from './route.model';
import { ApiRoutes } from 'src/utilities/api-routes';

@Injectable({
  providedIn: 'root'
})
export class GABestRouteService {
  constructor(private http: HttpClient) {
  }

  public CalculateBestRouteTest(places: Place[]): Route {
    var r = {
      id: "001",
      distance: 120.0,
      route: [
        { x: 26.7, y: 35.8, id: "001" } as Place,
        { x: 23.5, y: 35.4, id: "002" } as Place,
        { x: 24.1, y: 37.2, id: "003" } as Place,
        { x: 25.2, y: 39.6, id: "004" } as Place,
        { x: 26.3, y: 20.8, id: "005" } as Place,
        { x: 26.7, y: 35.8, id: "006" } as Place
      ]
    } as Route;

    var res = {
      id: "001",
      distance: 120.0,
      route: []
    } as Route;

    places.forEach(place => {
      res.route.push({ x: place.x, y: place.y, id: "001" } as Place);
    });

    return res;
  };

  public CalculateBestRoute(places: Place[]): Observable<Route> {
    var request = {
      places: []
    };

    places.forEach(place => {
      request.places.push(
        {
          x: place.x,
          y: place.y
        }
      )
    });

    return this.http.post<Route>("https://localhost:5001/calculations/bestroute", request);
  };
}
