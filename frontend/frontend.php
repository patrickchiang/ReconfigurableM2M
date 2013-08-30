<?php
/*
 * This page deals with frontend and filesystem/backend communications
 * Author: Patrick Chiang
 */

// For loading workspace saves
if ($_SERVER['REQUEST_METHOD'] == "GET" && isset($_GET["load"])) {
    echo file_get_contents("workspace_save.txt");
    die();
}

// Piping discovery information from backend to frontend
if ($_SERVER['REQUEST_METHOD'] == "GET" && isset($_GET["deploy"])) {
    $client = stream_socket_client("tcp://127.0.0.1:9001", $errno, $errorMessage);
    fwrite($client, "discover");
    $discovery = stream_get_contents($client);
    echo $discovery;
    fclose($client);
    die();
}

// Tell backend we are ready for deployment
if ($_SERVER['REQUEST_METHOD'] == "GET" && isset($_GET["done"])) {
    $client = stream_socket_client("tcp://127.0.0.1:9001", $errno, $errorMessage);
    fwrite($client, "deploy");
    fclose($client);
    echo "done";
    die();
}

// Save workspace to file
if ($_SERVER['REQUEST_METHOD'] == "POST") {
    $obj = file_get_contents("php://input");
    file_put_contents("workspace_save.txt", $obj);
    echo $obj;
    die();
}
?>