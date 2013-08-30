/*
* This contains all the scripts required to make the frontend work
* Author: Patrick Chiang
*/

// The html module blocks
var modules = new Array();

// The js module objects
var mods = new Array();

// The module types that enable value
var modulesWithValues = ["Number"];

// Current module ID for recursing
var moduleId;

window.onload = function() {
    $("#addModule").mousedown(addModule);
    $("#saveWorkspace").mousedown(saveWorkspace);
    $("#loadWorkspace").mousedown(loadWorkspace);
    $("#deploy").mousedown(deploy);
};

/*
 * Set up the plumbing: connections, connectors, endpoints..etc.
 */
function prepare(moduleName, inputNr, outputNr) {
    jsPlumb.importDefaults({
        // default drag options
        DragOptions : {
            cursor : "pointer",
            zIndex : 2000
        },
        EndpointStyles : [{
            fillStyle : "white"
        }, {
            fillStyle : "white"
        }],
        Endpoints : [["Dot", {
            radius : 7
        }], ["Dot", {
            radius : 9
        }]],
        ConnectionOverlays : [["Arrow", {
            location : 0,
            direction : -1
        }]]
    });

    // this is the paint style for the connecting lines..
    var connectorPaintStyle = {
        lineWidth : 4,
        strokeStyle : "#504A4B", //deea18 = yellow
        joinstyle : "round",
        outlineColor : "#EAEDEF",
        outlineWidth : 2
    },
    // .. and this is the hover style.
    connectorHoverStyle = {
        lineWidth : 4,
        strokeStyle : "white"
    }, endpointHoverStyle = {
        fillStyle : "#504A4B"
    },
    // the definition of source endpoints
    sourceEndpoint = {
        endpoint : "Dot",
        paintStyle : {
            strokeStyle : "#504A4B",
            fillStyle : "transparent",
            radius : 7,
            lineWidth : 2
        },
        isSource : true,
        connector : ["Flowchart", {
            stub : 30,
            gap : 0,
            cornerRadius : 5,
            alwaysRespectStubs : true
        }],
        cssClass : "endpointDragger",
        connectorStyle : connectorPaintStyle,
        hoverPaintStyle : endpointHoverStyle,
        connectorHoverStyle : connectorHoverStyle,
        dragOptions : {},
        overlays : [["Label", {
            location : [-1.0, -0.2],
            label : "Input",
            cssClass : "endpointLabel"
        }]]
    },

    // the definition of target endpoints (will appear when the user drags a connection)
    targetEndpoint = {
        endpoint : "Dot",
        paintStyle : {
            strokeStyle : "#504A4B",
            fillStyle : "transparent",
            radius : 9,
            lineWidth : 2
        },
        //hoverPaintStyle : endpointHoverStyle,
        maxConnections : 1,
        isTarget : true,
        overlays : [["Label", {
            location : [2.0, -0.2],
            label : "Output",
            cssClass : "endpointLabel"
        }]]
    };

    // Create the connections
    var allSourceEndpoints = [], allTargetEndpoints = [];
    _addEndpoints = function(toId, sourceAnchors, targetAnchors) {
        for (var i = 0; i < sourceAnchors.length; i++) {
            // convert from module save code -> actual connections
            var sourceUUID = toId + "x" + sourceAnchors[i].toString().replace(/\./g, "").replace(/\,/g, "").replace(/-/g, "");
            allSourceEndpoints.push(jsPlumb.addEndpoint(toId, sourceEndpoint, {
                anchor : sourceAnchors[i],
                uuid : sourceUUID
            }));
        }
        for (var j = 0; j < targetAnchors.length; j++) {
            var targetUUID = toId + "x" + targetAnchors[j].toString().replace(/\./g, "").replace(/\,/g, "").replace(/-/g, "");
            allTargetEndpoints.push(jsPlumb.addEndpoint(toId, targetEndpoint, {
                anchor : targetAnchors[j],
                uuid : targetUUID
            }));
        }
    };

    var oldEndpoints = jsPlumb.getEndpoints(moduleName);
    if (oldEndpoints !== undefined) {
        for (var i = 0; i < oldEndpoints.length; i++)
            jsPlumb.deleteEndpoint(oldEndpoints[i]);
    }

    // Evenly distribute locations of endpoints on module
    for (var i = 0; i < inputNr; i++) {
        _addEndpoints(moduleName, [[0, (i + 1) / (inputNr + 1), -1, 0]], []);
    }

    for (var i = 0; i < outputNr; i++) {
        _addEndpoints(moduleName, [], [[1, (i + 1) / (outputNr + 1), 1, 0]]);
    }

    // listen for clicks on connections to delete connections on click.
    jsPlumb.bind("click", function(conn, originalEvent) {
        jsPlumb.detach(conn);
        touched();
    });

    jsPlumb.bind("connection", function(info) {
        touched();
    });
}

/*
 * Clone a module
 */
function addModule() {
    var module = $("#template").clone();
    prepareModule(module);
}

/*
 * Send query to PHP file to load the workspace file
 */
function loadWorkspace() {
    // Intro
    $("#overlay div").html("Loading Workspace...");
    displayOverlay();

    // clear workspace
    $("#workspace").html("");
    jsPlumb.reset();

    $.ajax({
        type : "GET",
        url : "frontend.php?load=1"
    }).done(dataToWorkspace);
}

/*
 * Set the workspace up according to the save file
 */
function dataToWorkspace(data) {
    if (data == "") {
        alert("No saved workspace found.");
        $("#overlay").fadeOut(1000, "easeInOutExpo");
        return;
    }

    modules = [];
    // Empty the HTML modules blocks
    var mods = [];
    // Empty the JS module øbjects
    mods = JSON.parse(data);

    // reset positions & values
    for (var i = 0; i < mods.length; i++) {
        if (mods[i] == null) {
            modules[modules.length] = null;
            continue;
        }// ALWAYS remember to filter out possibly empty modules (deleted)
        var module = $("#template").clone();

        // reset positions
        module.css({
            left : mods[i].x,
            top : mods[i].y
        });

        // setup module
        prepareModule(module);

        // reset values
        var nameOfModule = module.attr("id");
        $("#" + nameOfModule + " .moduleType").data("selectBox-selectBoxIt").selectOption(mods[i].modType);
        $("#" + nameOfModule + " .moduleId").val("Module ID: " + mods[i].moduleId);
        $("#" + nameOfModule + " .deviceId").val(mods[i].deviceId);
        $("#" + nameOfModule + " .inputValue").val(mods[i].inputValue);

        // initialize module according to type
        selectModuleType.call($("#" + nameOfModule + " .moduleType"));
    }

    // connect endpoints AFTER all modules have their positions
    for (var i = 0; i < mods.length; i++) {
        if (mods[i] == null) {
            continue;
        }

        for (var j = 0; j < mods[i].conn.length; j++) {
            var pair = mods[i].conn[j];
            jsPlumb.connect({
                uuids : [pair.sourceId, pair.targetId],
                editable : true
            });
        }
    }

    $(".inputValue").each(function() {
        if ($(this).val() != "Input Value")
            $(this).removeClass("placeholder");
    });

    // Outro for "Loading..."
    $("#overlay").fadeOut(1000, "easeInOutExpo");
}

/*
 * Turn modules into JS objects and posts to PHP for save
 */
function saveWorkspace() {
    // Intro
    $("#overlay div").html("Saving Workspace...");
    displayOverlay();

    // blank out value of placeholders, no need to save
    $(this).find("[placeholder]").each(function() {
        var input = $(this);
        if (input.val() == input.attr("placeholder")) {
            input.val("");
        }
    })
    mods = [];
    // blank out JS objects

    // recreate module as a JS object
    for (var i = 0; i < modules.length; i++) {
        var mod = new Object();

        if (modules[i] == null) {
            mods.push(null);
            // create an empty slot in the array
            continue;
        }

        // get the full html name of the module
        var modName = modules[i].attr("id");

        // error! This disallows saving when modules don't have a proper type
        if ($("#" + modName + " .moduleType").val() == null) {
            $("#overlay div").html("Error: Some modules are not yet used!!");
            displayOverlay();
            $("#overlay").fadeOut(1000, "easeInOutExpo");
            return;
        }

        // pixels from left
        mod.x = $("#" + modName).css("left");
        // pixels from top
        mod.y = $("#" + modName).css("top");
        // module type
        mod.modType = $("#" + modName + " .moduleType").val();
        // clear module ID
        mod.moduleId = "";
        // device ID
        mod.deviceId = $("#" + modName + " .deviceId").val();
        // value
        mod.inputValue = $("#" + modName + " .inputValue").val();

        // connections
        mod.conn = new Array();
        var connections = jsPlumb.getConnections({
            source : [modName]
        });
        for (var j = 0; j < connections.length; j++) {
            var pair = new Object();
            // source point
            pair.sourceId = connections[j].endpoints[0].getUuid();
            //  target point
            pair.targetId = connections[j].endpoints[1].getUuid();
            mod.conn.push(pair);
        }

        // detects whether module is leftmost (orphan)
        var outputs = jsPlumb.getConnections({
            target : [modName]
        });
        if (outputs.length == 0) {
            mod.output = null;
        } else {
            mod.output = "placeholder";
        }

        // this is just one module
        mods.push(mod);
    }

    // start working with module 0
    moduleId = 0;

    for (var i = 0; i < mods.length; i++) {
        if (mods[i] == null) {
            continue;
        }
        // it's an orphan
        if (mods[i].output == null) {
            recursiveLayering(mods, i);
        }
    }

    // set up the module id
    for (var i = 0; i < modules.length; i++) {
        if (modules[i] == null) {
            continue;
        }

        var modName = modules[i].attr("id");
        mods[i].moduleId = $("#" + modName + " .moduleId").val();
        $("#" + modName + " .moduleId").val("Module ID: " + mods[i].moduleId);
    }

    // set up all the remaining connections
    for (var i = 0; i < mods.length; i++) {
        if (modules[i] == null) {
            continue;
        }

        var mod = mods[i];
        var modName = modules[i].attr("id");
        var outputs = jsPlumb.getConnections({
            target : [modName]
        });
        if (outputs.length == 0) {
            mod.output = null;
        } else {
            var modId = outputs[0].endpoints[0].getUuid().split("x")[0].split("module")[1];
            mod.output = mods[modId].moduleId;
        }
    }

    // JS objects array -> JSON
    var jsonData = JSON.stringify(mods);

    $.ajax({
        type : "POST",
        url : "frontend.php",
        data : jsonData,
    }).done(function(data) {
        $("#overlay").fadeOut(1000, "easeInOutExpo");
    });
}

/*
 * Determines the module ID
 */
function recursiveLayering(mods, index) {
    // go down the whole list, recursively to the end
    for (var i = 0; i < mods[index].conn.length; i++) {
        var previousIndex = mods[index].conn[i].targetId.split("x")[0].split("module")[1];
        if (isNumber(mods[previousIndex]) && previousIndex < moduleId)
            continue;
        recursiveLayering(mods, previousIndex);
    }

    // number the module
    var modName = modules[index].attr("id");
    $("#" + modName + " .moduleId").val(moduleId);
    moduleId++;
}

/*
 * Request backend for discovery information for modules
 */
function deploy() {
    saveWorkspace();

    // Intro
    $("#overlay div").html("Deploying Modules...");
    displayOverlay();

    var discoveryFile = "";
    $.ajax({
        type : "GET",
        url : "frontend.php?deploy=1"
    }).done(function(data) {
        //alert(data);
        discoveryFile = data;
        // $("#overlay").fadeOut(1000, "easeInOutExpo");

        // Now, use discovery data to order & construct my JSON file
        var devices = discoveryFile.split("\n");

        // * Assign device ids to modules *

        // Loop through key
        for (var i = 0; i < mods.length; i++) {
            if (mods[i] == null) {
                continue;
            }

            // Don't stop until a device is found
            var found = false;
            for (var j = 0; j < 5000; j++) {
                var key = getRandomInt(0, devices.length - 1);
                if (devices == null || devices[key].trim().replace("<br>", "") == "") {
                    continue;
                    // moving on
                }
                var device = JSON.parse(devices[key].replace("\n", ""));
                if (~device.capabilities.indexOf(mods[i].modType)) {
                    $("#module" + i + " .deviceId").val("Device: " + device.id);
                    found = true;
                    break;
                    // break out of infinite loop
                }
            }

            // if device capabilities are not found
            if (!found) {
                $("#overlay div").html("System lacks capabilities necessary...");
                displayOverlay();
                //$("#overlay").fadeOut(1000, "easeInOutExpo");
            }
        }

        // save again with deployment id info
        saveWorkspace();

        prepareJson();
    });
}

/*
 * Prepare a heartbeat and post to file
 */
function prepareHeartbeat() {
    // Intro
    $("#overlay div").html("Preparing Heartbeat...");
    displayOverlay();

    var heartbeat = [];
    for (var i = 0; i < mods.length; i++) {
        if (mods[i] == null) {
            continue;
        }

        // get all orphan modules
        if (mods[i].conn.length == 0) {
            var pulse = new Object();
            pulse.moduleId = mods[i].moduleId;
            pulse.inputModuleId = "0";
            pulse.instanceId = "0";
            pulse.value = mods[i].inputValue;
            pulse.deviceId = mods[i].deviceId.split("Device: ")[1];

            heartbeat.push(pulse);
        }
    }

    $.ajax({
        type : "POST",
        url : "heartbeat.php",
        data : JSON.stringify(heartbeat)
    }).done(function(data) {
        // Tell backend we're done
        $.ajax({
            type : "GET",
            url : "frontend.php?done=1"
        }).done(function(data) {
            // Outro, last thing after deployment
            //$("#overlay").fadeOut(1000, "easeInOutExpo");
        });
    });
}

/*
 * Prepare JSON for deployment and posts to file
 */
function prepareJson() {
    // Intro
    $("#overlay div").html("Preparing Deployment...");
    displayOverlay();

    var deployJson = [];
    for (var i = 0; i < mods.length; i++) {
        if (mods[i] == null)
            continue;
        var deployObj = new Object();
        deployObj.moduleId = mods[i].moduleId;
        deployObj.instanceId = "0";
        deployObj.output = mods[i].output || "";

        if (deployObj.output == "") {
            deployObj.output = "end";
        }

        deployObj.inputs = [];
        for (var j = 0; j < mods[i].conn.length; j++) {
            var modId = mods[i].conn[j].targetId.split("x")[0].split("module")[1];
            deployObj.inputs.push(mods[modId].moduleId);
        }

        deployObj.values = [];
        if (mods[i].inputValue != "Input Value") {
            deployObj.values.push(mods[i].inputValue);
        }

        deployObj.deviceId = mods[i].deviceId.split("Device: ")[1];
        deployObj.name = mods[i].modType;
        deployJson.push(deployObj);
    }

	$.ajax({
		type : "POST",
		url : "deploy.php",
		data : JSON.stringify(deployJson)
	}).done(function(data) {
		prepareHeartbeat();
	});
}

/*
 * Creates a module, set it up, and pushes it into the main array
 */
function prepareModule(module) {
    // name the module
    var nameOfModule = "module" + modules.length;
    module.attr({
        "id" : nameOfModule,
        "class" : "module"
    });

    // it floats when you hover your mouse over it
    module.hover(function() {
        $("[id^=module]").css("z-index", "-10");
        module.css("z-index", "10");
    });

    // current length of modules array
    var moduleIndex = modules.length;

    // add module to workspace
    modules[moduleIndex] = module;
    module.appendTo("#workspace");

    // make draggable
    jsPlumb.draggable(module, {
        //containment: "#workspace"
    });

    // module type & select box setup
    var modType = $("#" + nameOfModule + " .moduleType");
    modType.selectBoxIt({
        showFirstOption : false,
        showEffect : "fadeIn",
        showEffectSpeed : 400,
        hideEffect : "fadeOut",
        hideEffectSpeed : 400,
        autoWidth : false
    });

    // handle when the value is changed
    modType.change(selectModuleType);

    // kill with fire when close button is clicked
    // (but don't delete its space, the space remains)
    $("#" + nameOfModule + " .closeBtn").click(function() {
        var oldEndpoints = jsPlumb.getEndpoints(nameOfModule);
        if (oldEndpoints !== undefined) {
            for (var i = 0; i < oldEndpoints.length; i++)
                jsPlumb.deleteEndpoint(oldEndpoints[i]);
        }

        $("#" + nameOfModule).remove();
        modules[moduleIndex] = null;
        //.splice(moduleIndex, 1);
    });

    touched();
}

/*
 * Called when the module type has been selected
 */
function selectModuleType() {
    var nameOfModule = $(this).parent().parent().attr("id");
    var selectedOption = "#" + nameOfModule + " .moduleType";

    // call prepare, give it the correct endpoints
    prepare(nameOfModule, $(selectedOption + " option:selected").data("inputs"), $(selectedOption + " option:selected").data("outputs"));

    // for disabling/enabling value box
    if (!jQuery.inArray($(this).val(), modulesWithValues)) {
        $("#" + nameOfModule + " .inputValue").removeAttr("disabled");
    } else {
        $("#" + nameOfModule + " .inputValue").attr("disabled", "disabled");
    }

    placeholder();
    touched();
}

/*
 * Shows placeholder before you add in information
 */
function placeholder() {
    $("[placeholder]").focus(function() {
        var input = $(this);
        if (input.val() == input.attr("placeholder")) {
            input.val("");
            input.removeClass("placeholder");
        }
    }).blur(function() {
        var input = $(this);
        if (input.val() == "" || input.val() == input.attr("placeholder")) {
            input.addClass("placeholder");
            input.val(input.attr("placeholder"));
        }
    }).blur();
}

/*
 * Fire when something has been changed since last deployment
 */
function touched() {

}

/*
 * Display the dark overlay
 */
function displayOverlay() {
    $("#overlay").css("display", "table");
}

/*
 * Is n a number?
 */
function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

/*
 * Get random integer between min and max
 */
function getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}