function displayResize()
{
    const login = location.pathname.substring(location.pathname.lastIndexOf('/') + 1) === 'Login'

    const mobile = window.innerHeight - window.innerWidth > 0 || window.innerWidth < 851

    const titleElement = document.getElementById('login-header-title')

    if (titleElement instanceof (HTMLHeadingElement))
    {
        titleElement.style.display = mobile ? 'none' : 'block'
    }
    const loginFormElement = document.getElementById('account-login-form')

    if (loginFormElement instanceof (HTMLDivElement))
    {
        loginFormElement.style.display = mobile ? 'none' : 'block'
    }
    const loginButtonContainer = document.getElementById('external-login-button-container')

    if (loginButtonContainer instanceof (HTMLDivElement))
    {
        const hr = loginButtonContainer.getElementsByTagName('hr')

        if (hr instanceof (HTMLCollection) && mobile && login)
        {
            for (let i = 0; i < hr.length; i++)
            {
                hr[i].remove()
            }
        }
        loginButtonContainer.style.textAlign = 'center'
    }
    document.getElementById('account-header').style.display = mobile ? 'none' : 'block'

    const accContainer = document.getElementById('account-container')

    if (accContainer instanceof (HTMLDivElement))
    {
        accContainer.style.paddingTop = mobile ? '1rem' : ''
        accContainer.style.height = mobile && login ? '97.25vh' : ''

        const h1 = accContainer.getElementsByTagName('h1')

        if (h1 instanceof (HTMLCollection) && mobile && login)
        {
            for (let i = 0; i < h1.length; i++)
            {
                h1[i].remove()
            }
        }
        if (accContainer.hasChildNodes())
        {
            accContainer.childNodes.forEach(e =>
            {
                if (e.nodeType === 3)
                {
                    return
                }
                changeTheAttributesChildren(e)

                e.style.height = 'inherit'
            })
        }
        const externalAcc = document.getElementById('external-account')

        if (externalAcc instanceof (HTMLFormElement))
        {
            externalAcc.style.display = mobile && login ? 'table' : 'block'
            externalAcc.style.width = mobile && login ? '100%' : 'auto'

            if (externalAcc.hasChildNodes())
            {
                externalAcc.childNodes.forEach(e =>
                {
                    if (e instanceof (HTMLDivElement))
                    {
                        e.style.display = mobile && login ? 'table-cell' : 'block'
                        e.style.verticalAlign = 'middle'
                    }
                })
            }
        }
    }
    document.body.style.backgroundRepeat = login ? 'no-repeat' : ''
    document.body.style.backgroundSize = login ? 'cover' : ''
    document.body.style.backgroundImage = mobile && login ? 'url("/images/techno-valley.png")' : ''
}
function changeTheAttributesChildren(node)
{
    if (node.hasChildNodes())
    {
        node.childNodes.forEach(e =>
        {
            if (e.nodeType === 3)
            {
                return
            }
            e.style.height = 'inherit'

            if (e instanceof (HTMLFormElement))
            {
                return
            }
            if (e.hasChildNodes())
            {
                changeTheAttributesChildren(e)
            }
        })
    }
}