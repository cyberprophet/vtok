function initializeIntroMap(id, loaderOptions, mapOptions)
{
    new google.maps.plugins.loader.Loader(loaderOptions).loadCallback(error =>
    {
        if (error)
        {
            console.log(error)
        }
        else
        {
            const div = document.getElementById(id)

            if (div instanceof (HTMLDivElement))
            {
                map = new google.maps.Map(div, mapOptions)

                map.addListener('click', () =>
                {
                    const center = map.getCenter()

                    console.log({
                        lat: center.lat(),
                        lng: center.lng()
                    })
                })
                goAnywhere(mapOptions.cameraOptions)

                function animate(time)
                {
                    requestAnimationFrame(animate)

                    TWEEN.update(time)
                }
                requestAnimationFrame(animate)
            }
        }
    })
}
function goAnywhere(cameraOptions, destination)
{
    if (map instanceof (google.maps.Map))
    {
        new TWEEN.Tween(cameraOptions)
            .to({
                center: {
                    lat: destination === undefined ? crd?.latitude : destination.latitude,
                    lng: destination === undefined ? crd?.longitude : destination.longitude
                },
                tilt: 71.5,
                heading: 35,
                zoom: 17
            },
                21 * 1024)
            .interpolation(TWEEN.Interpolation.Bezier)
            .easing(TWEEN.Easing.Sinusoidal.Out)
            .onUpdate(() => map.moveCamera(cameraOptions))
            .delay(512)
            .start()
    }
}
function getInstanceofDivElement(id)
{
    const div = document.getElementById(id)

    return div === null || div === undefined
}
function setCoordinate()
{
    if (navigator.geolocation)
    {
        navigator.geolocation.getCurrentPosition(p =>
        {
            crd = p.coords
        },
            e => console.log(e),
            {
                enableHighAccuracy: true
            })
    }
}
function getCoordinate()
{
    console.log(crd)

    return JSON.stringify({
        lng: crd.longitude,
        lat: crd.latitude
    })
}
function getThemeColor()
{
    if (window.matchMedia)
    {
        const theme = window.matchMedia('(prefers-color-scheme: dark)').matches

        console.log(theme)

        return theme
    }
    return false
}
let map, crd, recall, markers, cluster, members, markerCluster