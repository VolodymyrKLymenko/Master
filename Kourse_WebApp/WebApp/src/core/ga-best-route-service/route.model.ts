export class Route {
    id: string;
    distance: number;
    durationInMilliseconds: number;
    generationsCount: number;
    route: Place[];
}

export class Place {
    id: string;
    x: number;
    y: number;
}
