export function initializeGoldenLayout(container, dotNetHelper) {
    var defaultConfig = {
        settings: {
            selectionEnabled: true
        },
        content: [{
            type: 'row',
            content: []
        }]
    };
    
    
    var goldenLayoutsavedState = localStorage.getItem( 'goldenLayoutSavedState' );
    
    if (goldenLayoutsavedState !== null) {
        defaultConfig = JSON.parse(goldenLayoutsavedState);
    }
    
    window.myLayout = new GoldenLayout(defaultConfig, container);

    window.dotNetHelper = dotNetHelper;

    myLayout.registerComponent('blazorComponent', function (container, state) {
        let componentId = `blazor-component-${Date.now()}`;
        let componentDiv = document.createElement('div');
        componentDiv.classList.add('p-4');
        componentDiv.id = componentId;
        container.getElement().append(componentDiv);

        // // ✅ Use ResizeObserver to track size changes dynamically
        // const resizeObserver = new ResizeObserver(entries => {
        //     for (let entry of entries) {
        //         let { width, height } = entry.contentRect;
        //         componentDiv.style.width = `${width}px`;
        //         componentDiv.style.height = `${height}px`;
        //     }
        // });
        //
        // // ✅ Observe changes in the Golden Layout container
        // let containerElement = container.getElement()[0];
        // resizeObserver.observe(containerElement);
        //
        // // ✅ Clean up observer when component is destroyed
        // container.on('destroy', function () {
        //     resizeObserver.unobserve(containerElement);
        // });

        window.renderBlazorComponent(componentDiv, state.component, state.parameters);
    });


    myLayout.resizeWithContainerAutomatically = true;
    
    myLayout.on('stateChanged', function() {
        localStorage.setItem('goldenLayoutSavedState', JSON.stringify(myLayout.toConfig()));
    });

    myLayout.init();


}

export function updateGoldenLayoutSize() {
    if (window.myLayout) {
        window.myLayout.updateSize();
    }
}

export function addGoldenLayoutComponent(component, title, parameters) {
    const newItemConfig = {
        type: 'component',
        componentName: 'blazorComponent',
        title: title,
        componentState: {
            component: component,
            text: 'test',
            parameters: parameters
        }
    };

    if (window.myLayout) {
        if (window.myLayout.selectedItem === null && window.myLayout.root.contentItems.length === 0) {
            window.myLayout.root.addChild(newItemConfig);
        }
        else if (window.myLayout.selectedItem === null) {
            window.myLayout.root.contentItems[0].addChild(newItemConfig);
        }
        else {
            window.myLayout.selectedItem.addChild(newItemConfig);
        }
    }
}

window.renderBlazorComponent = async (element, componentType, parameters) => {
    // await Blazor.rootComponents.add(selector, componentType, {});
    // let containerElement = document.getElementById(selector);
    await Blazor.rootComponents.add(element, componentType, parameters);
};

// var persistentComponent = function( container, state ){
//
//     //Check for localStorage
//     if( !typeof window.localStorage ) {
//         container.getElement().append(  '<h2 class="err">Your browser doesn\'t support localStorage.</h2>');
//         return;
//     }
//
//     // Create the input
//     var input = $( '<input type="text" />' );
//
//     // Set the initial / saved state
//     if( state.label ) {
//         input.val( state.label );
//     }
//
//     // Store state updates
//     input.on( 'change', function(){
//         container.setState({
//             label: input.val()
//         });
//     });
//
//     // Append it to the DOM
//     // container.getElement().append(  '<h2>I\'ll be saved to localStorage</h2>', input );
// };

export function initializeMainLayout () {
    const layoutConfig = {
        content: [{
            type: "row",
            content: [
                {
                    type: "component",
                    componentName: "blazorComponent",
                    title: "Left Sidebar",
                    componentState: { componentName: "Osmo.Components.LeftSidebar" }
                },
                {
                    type: "column",
                    content: [
                        {
                            type: "component",
                            componentName: "blazorComponent",
                            title: "Main Content",
                            componentState: { componentName: "Osmo.Components.GoldenLayout" }
                        },
                        {
                            type: "component",
                            componentName: "blazorComponent",
                            title: "Right Sidebar",
                            componentState: { componentName: "Osmo.Components.RightSidebar" }
                        }
                    ]
                }
            ]
        }]
    };

    const layout = new GoldenLayout(layoutConfig, document.getElementById("golden-layout-container"));

    layout.registerComponent("blazorComponent", function (container, state) {
        let componentId = `blazor-comp-${Date.now()}`;
        let div = document.createElement("div");
        div.id = componentId;
        div.style.width = "100%";
        div.style.height = "100%";
        container.getElement().append(div);

        // Call Blazor to render the component
        // DotNet.invokeMethodAsync("YourBlazorAppNamespace", "RenderBlazorComponent", state.componentName, componentId);
    });

    layout.init();
}

