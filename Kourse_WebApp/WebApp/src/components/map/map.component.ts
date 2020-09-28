import { Component, OnInit } from '@angular/core';

import { Place, Route } from 'src/core/ga-best-route-service/route.model';
import { StatisticEntity } from 'src/core/models/statistic.model';
import { GABestRouteService } from 'src/core/ga-best-route-service/gabestroute.service';
import { LocationService } from 'src/core/location.service';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.styl']
})
export class MapComponent implements OnInit {
    public mapInitialized = false;
    public markerIconUrl: string;
    public options: any;
    public overlays: any[];
    public route: Route;
    public statisticEntities: StatisticEntity[] = [];
  
    private calculationCount = 0;
    private tempPlaces = []

  constructor(
    private locationService: LocationService,
    private gaBestRouteCalculation: GABestRouteService) { }

  public ngOnInit(): void {
    this.options = {
        zoom: 12
      };

    this.initializeLocation();

    this.overlays = [];
  }

  private initializeLocation(): void {
    this.markerIconUrl = this.locationService.getMapIconUrl();

    if (this.locationService.userLatitude === null || this.locationService.userLongitude === null) {
        this.locationService.initializeLocation();
    }

    window.navigator.geolocation.getCurrentPosition(location => {
      this.options.center = {
        lat: location.coords.latitude,
        lng: location.coords.longitude
      }

      this.mapInitialized = true;
    });
  }

  public calculateRoute(): void {
    this.gaBestRouteCalculation.CalculateBestRoute(this.tempPlaces)
      .subscribe(route => {
        this.route = route;

        this.calculationCount = this.calculationCount + 1;
        this.statisticEntities.push(
          {
            id: this.calculationCount,
            distance: this.route.distance,
            durationInMilliseconds: this.route.durationInMilliseconds,
            generationsCount: this.route.generationsCount
          } as StatisticEntity
        )

        this.resetRoute();

        this.writeRoute();
      });
  }

  private writeRoute(): void {
    for (let index = 0; index < this.route.route.length - 1; index++) {
      var place1 = this.route.route[index];
      var place2 = this.route.route[index + 1];

      console.log(place1, place2);

      this.overlays.push(
        new google.maps.Polyline({
          path: [
            {
              lat: place1.x,
              lng: place1.y
            },
            {
              lat: place2.x,
              lng: place2.y
            }
          ],
          geodesic: true,
          strokeColor: '#FF0000',
          strokeOpacity: 0.5,
          strokeWeight: 2
        })
      );
    }

    place1 = this.route.route[0];
    place2 = this.route.route[this.route.route.length-1];

    this.overlays.push(
      new google.maps.Polyline({
        path: [
          {
            lat: place1.x,
            lng: place1.y
          },
          {
            lat: place2.x,
            lng: place2.y
          }
        ],
        geodesic: true,
        strokeColor: '#FF0000',
        strokeOpacity: 0.5,
        strokeWeight: 2
      })
    );
  }

  public handleMapClick(event): void {
    this.overlays.push(
      new google.maps.Marker(
      {
        position: {lat: event.latLng.lat(), lng: event.latLng.lng()},
        title:"Konyaalti",
        icon: this.markerIconUrl
      })
    );

    this.tempPlaces.push(
      { x: event.latLng.lat(), y: event.latLng.lng(), id: "001" } as Place,
    );
  }

  public handleOverlayClick(event): void {
    const index: number = this.tempPlaces.findIndex(
      (element) => element.x == event.overlay.position.lat() && element.y == event.overlay.position.lng());
    
    if (index !== -1) {
      this.tempPlaces.splice(index, 1);
    }

    this.resetRoute();
  }

  public resetPlaces(): void {
    this.tempPlaces = [];

    this.resetRoute();
  }

  private resetRoute(): void {
    this.overlays = [];
    this.tempPlaces.forEach(place => {
      this.overlays.push(
        new google.maps.Marker({
          position: {
            lat: place.x,
            lng: place.y
          },
          title: `${place.x},${place.y}`,
          icon: this.markerIconUrl
        })
      )
    });
  }
}
