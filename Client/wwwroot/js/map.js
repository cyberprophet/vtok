function markersOnTheMap(geo, args)
{
    if (args instanceof (Array))
    {
        args.forEach((value, index, arr) =>
        {
            marker = new google.maps.Marker({
                icon:
                {
                    url: value.png,
                    scaledSize: new google.maps.Size(37.5, 37.5),
                    labelOrigin: new google.maps.Point((value.name.length - 1) / 2 * -14.5, 25)
                },
                optimized: true,
                position: value.position,
                animation: google.maps.Animation.DROP,
                title: value.code
            })
            marker.addListener('mouseover', e =>
            {
                const m = markers.find(o => o.position === e.latLng)

                if (m instanceof (google.maps.Marker))
                {
                    if (m.getLabel() === null || m.getLabel() === undefined)
                    {
                        m.setLabel({
                            text: value.name,
                            fontWeight: '900',
                            fontSize: '15px',
                            fontFamily: 'Consolas'
                        })
                        if (map.minZoom > 7)
                        {
                            map.setOptions({
                                minZoom: map.minZoom - 1
                            })
                        }
                    }
                }
            })
            markers.push(marker)
        })
        cluster = new markerClusterer.MarkerClusterer({
            clusterOptions:
            {
                averageCenter: false,
                zoomOnClick: true
            },
            markers,
            map
        })
        if (map instanceof (google.maps.Map) &&
            geo != undefined && geo.lng != 0 && geo.lat != 0)
        {
            map.panTo(geo)
        }
    }
    members = []
}
function markMembersOnTheMap(args)
{
    if (args instanceof (Array))
    {
        args.forEach((value, index, arr) =>
        {
            member = new google.maps.Marker({
                icon:
                {
                    url: value.png,
                    scaledSize: new google.maps.Size(51.75, 51.75),
                    labelOrigin: new google.maps.Point((value.name.length - 1) / 2 * -7.95, 25)
                },
                optimized: true,
                position: value.position,
                animation: google.maps.Animation.DROP,
                title: value.code
            })
            member.addListener('mouseover', e =>
            {
                const m = members.find(o => o.position === e.latLng)

                if (m instanceof (google.maps.Marker))
                {
                    if (m.getLabel() === null || m.getLabel() === undefined)
                    {
                        m.setLabel({
                            text: value.name.toUpperCase(),
                            fontWeight: '900',
                            fontSize: '15px',
                            fontFamily: 'Consolas'
                        })
                    }
                }
            })
            members.push(member)
        })
        markerCluster = new markerClusterer.MarkerClusterer({
            clusterOptions:
            {
                averageCenter: false,
                zoomOnClick: true
            },
            renderer: {
                render: ({ count, position }, stats) =>
                {
                    const color = count > Math.max(5, stats.clusters.markers.mean) ? "#00ff00" : "#ffd700"

                    const svg = window.btoa(`<svg fill="${color}" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 240 240">
                                                <circle cx="120" cy="120" opacity=".6" r="70" />
                                                <circle cx="120" cy="120" opacity=".3" r="90" />
                                                <circle cx="120" cy="120" opacity=".2" r="110" />
                                            </svg>`)

                    return new google.maps.Marker({
                        icon: {
                            url: `data:image/svg+xml;base64,${svg}`,
                            scaledSize: new google.maps.Size(55, 55)
                        },
                        label: {
                            text: String(count),
                            color: "rgba(255,255,255,0.9)",
                            fontSize: "19px",
                            fontFamily: 'Consolas'
                        },
                        zIndex: Number(google.maps.Marker.MAX_ZINDEX) + count,
                        title: `Cluster of ${count} members`,
                        position
                    })
                }
            },
            markers: members,
            map
        })
    }
}
function initialize(id, loaderOptions, center, reference)
{
    new google.maps.plugins.loader.Loader(loaderOptions).loadCallback(error =>
    {
        if (error)
        {
            console.log(error)
        }
        else
        {
            recall = reference

            const div = document.getElementById(id)

            if (div instanceof (HTMLDivElement))
            {
                map = new google.maps.Map(div, {
                    center: center,
                    zoomControl: false,
                    fullscreenControl: false,
                    mapTypeControl: false,
                    minZoom: 13,
                    zoom: 17,
                    mapTypeId: 'terrain',
                    styles: styleOfBright
                })
                map.addListener('zoom_changed',
                    () => recall.invokeMethodAsync(state, 'zoom_changed'))

                map.addListener('idle',
                    () => recall.invokeMethodAsync(state, 'idle'))
            }
        }
    })
    markers = []
}
const styleOfDark =
    [{
        elementType: "geometry",
        stylers: [
            {
                color: "#242f3e"
            }
        ]
    },
    {
        elementType: "labels.text.stroke",
        stylers: [
            {
                color: "#242f3e"
            }
        ]
    },
    {
        elementType: "labels.text.fill",
        stylers: [
            {
                color: "#746855"
            }
        ]
    },
    {
        featureType: "administrative.locality",
        elementType: "labels.text.fill",
        stylers: [
            {
                color: "#d59563"
            }
        ]
    },
    {
        featureType: "poi",
        elementType: "labels.text.fill",
        stylers: [
            {
                color: "#d59563"
            }
        ]
    },
    {
        featureType: "poi.park",
        elementType: "geometry",
        stylers: [
            {
                color: "#263c3f"
            }
        ]
    },
    {
        featureType: "poi.park",
        elementType: "labels.text.fill",
        stylers: [
            {
                color: "#6b9a76"
            }
        ]
    },
    {
        featureType: "road",
        elementType: "geometry",
        stylers: [
            {
                color: "#38414e"
            }
        ]
    },
    {
        featureType: "road",
        elementType: "geometry.stroke",
        stylers: [
            {
                color: "#212a37"
            }
        ]
    },
    {
        featureType: "road",
        elementType: "labels.text.fill",
        stylers: [
            {
                color: "#9ca5b3"
            }
        ]
    },
    {
        featureType: "road.highway",
        elementType: "geometry",
        stylers: [
            {
                color: "#746855"
            }
        ]
    },
    {
        featureType: "road.highway",
        elementType: "geometry.stroke",
        stylers: [
            {
                color: "#1f2835"
            }
        ]
    },
    {
        featureType: "road.highway",
        elementType: "labels.text.fill",
        stylers: [
            {
                color: "#f3d19c"
            }
        ]
    },
    {
        featureType: "transit",
        elementType: "geometry",
        stylers: [
            {
                color: "#2f3948"
            }
        ]
    },
    {
        featureType: "transit.station",
        elementType: "labels.text.fill",
        stylers: [
            {
                color: "#d59563"
            }
        ]
    },
    {
        featureType: "water",
        elementType: "geometry",
        stylers: [
            {
                color: "#17263c"
            }
        ]
    },
    {
        featureType: "water",
        elementType: "labels.text.fill",
        stylers: [
            {
                color: "#515c6d"
            }
        ]
    },
    {
        featureType: "water",
        elementType: "labels.text.stroke",
        stylers: [
            {
                color: "#17263c"
            }
        ]
    }]
const styleOfBright =
    [{
        elementType: 'geometry',
        stylers: [
            {
                color: '#f5f5f5'
            }
        ]
    },
    {
        elementType: 'labels.icon',
        stylers: [
            {
                visibility: 'off'
            }
        ]
    },
    {
        elementType: 'labels.text.fill',
        stylers: [
            {
                color: '#616161'
            }
        ]
    },
    {
        elementType: 'labels.text.stroke',
        stylers: [
            {
                color: '#f5f5f5'
            }
        ]
    },
    {
        featureType: 'administrative.land_parcel',
        elementType: 'labels.text.fill',
        stylers: [
            {
                color: '#bdbdbd'
            }
        ]
    },
    {
        featureType: 'poi',
        elementType: 'geometry',
        stylers: [
            {
                color: '#eeeeee'
            }
        ]
    },
    {
        featureType: 'poi',
        elementType: 'labels.text.fill',
        stylers: [
            {
                color: '#757575'
            }
        ]
    },
    {
        featureType: 'poi.park',
        elementType: 'geometry',
        stylers: [
            {
                color: '#e5e5e5'
            }
        ]
    },
    {
        featureType: 'poi.park',
        elementType: 'labels.text.fill',
        stylers: [
            {
                color: "#9e9e9e"
            }
        ]
    },
    {
        featureType: "road",
        elementType: "geometry",
        stylers: [
            {
                color: "#ffffff"
            }
        ]
    },
    {
        featureType: "road.arterial",
        elementType: "labels.text.fill",
        stylers: [
            {
                color: "#757575"
            }
        ]
    },
    {
        featureType: "road.highway",
        elementType: "geometry",
        stylers: [
            {
                color: "#dadada"
            }
        ]
    },
    {
        featureType: "road.highway",
        elementType: "labels.text.fill",
        stylers: [
            {
                color: "#616161"
            }
        ]
    },
    {
        featureType: "road.local",
        elementType: "labels.text.fill",
        stylers: [
            {
                color: "#9e9e9e"
            }
        ]
    },
    {
        featureType: "transit.line",
        elementType: "geometry",
        stylers: [
            {
                color: "#e5e5e5"
            }
        ]
    },
    {
        featureType: "transit.station",
        elementType: "geometry",
        stylers: [
            {
                color: "#eeeeee"
            }
        ]
    },
    {
        featureType: "water",
        elementType: "geometry",
        stylers: [
            {
                color: "#c9c9c9"
            }
        ]
    },
    {
        featureType: "water",
        elementType: "labels.text.fill",
        stylers: [
            {
                color: "#9e9e9e"
            }
        ]
    }]
const state = 'StateHasChanged'