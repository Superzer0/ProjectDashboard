
var app = angular.module('angularAdmin',
    ['dashboardAuth', 'ngRoute', 'LocalStorageModule', 'angular-loading-bar', 'growlNotifications',
        'ngAnimate', 'frapontillo.bootstrap-switch', 'angularFileUpload', 'ng.jsoneditor']);

app.config(['$routeProvider', 'cfpLoadingBarProvider', function ($routeProvider, cfpLoadingBarProvider) {
    cfpLoadingBarProvider.includeSpinner = false;
    cfpLoadingBarProvider.latencyThreshold = 5;

    $routeProvider.when("/home", {
        controller: "homeController",
        templateUrl: "/app/views/home.html"
    });

    $routeProvider.when("/login", {
        controller: "loginController",
        templateUrl: "/app/views/login.html"
    });

    $routeProvider.when("/signup", {
        controller: "signupController",
        templateUrl: "/app/views/signup.html"
    });

    $routeProvider.when("/refresh", {
        controller: "refreshController",
        templateUrl: "/app/views/refresh.html"
    });

    //---- user

    $routeProvider.when("/user/index", {
        controller: "indexUserController",
        templateUrl: "/app/views/user/index.html",
        resolve: {
            userProfile: ['authService', function (authService) {
                return authService.getCurrentUserProfile();
            }]
        }
    });

    $routeProvider.when("/user/plugins", {
        controller: "userPluginsController",
        templateUrl: "/app/views/user/plugins.html"
    });

    $routeProvider.when("/user/plugins/:id/:version", {
        controller: "userPluginConfigurationController",
        templateUrl: "/app/views/user/configure.html"
    });

    //---- instance

    $routeProvider.when("/instance/users", {
        controller: "instanceUsersController",
        templateUrl: "/app/views/instance/users.html"
    });

    $routeProvider.when("/instance/index", {
        controller: "brokerInstanceController",
        templateUrl: "/app/views/instance/broker.html"
    });

    $routeProvider.when("/instance/tokens", {
        controller: "appsManagerController",
        templateUrl: "/app/views/instance/tokens.html"
    });

    //---- plugins

    $routeProvider.when("/plugins", {
        controller: "indexPluginsController",
        templateUrl: "/app/views/plugins/index.html"
    });

    $routeProvider.when("/plugins/:id/:version", {
        controller: "pluginController",
        templateUrl: "/app/views/plugins/plugin.html"
    });

    $routeProvider.when("/plugins/upload", {
        controller: "indexPluginsController",
        templateUrl: "/app/views/plugins/upload.html"
    });

    $routeProvider.otherwise({ redirectTo: "/home" });

}]);

app.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('dashboardAuthMiddlewareInterceptor');
    $httpProvider.defaults.withCredentials = true;
}]);

app.run(['authService', function (authService) {
    authService.fillAuthData();
}]);


