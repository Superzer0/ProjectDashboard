
var app = angular.module('angularAdmin',
    ['ngRoute', 'LocalStorageModule', 'angular-loading-bar', 'growlNotifications',
        'ngAnimate', 'frapontillo.bootstrap-switch', 'angularFileUpload']);

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

    $routeProvider.when("/tokens", {
        controller: "tokensManagerController",
        templateUrl: "/app/views/tokens.html"
    });

    //---- user

    $routeProvider.when("/user/index", {
        controller: "indexUserController",
        templateUrl: "/app/views/user/index.html",
        resolve: {
            userProfile: ['authService', function (authService) {
                return authService.getUserProfile();
            }]
        }
    });

    $routeProvider.when("/user/plugins", {
        controller: "indexUserController",
        templateUrl: "/app/views/user/plugins.html"
    });

    //---- instance

    $routeProvider.when("/instance/index", {
        controller: "indexInstanceController",
        templateUrl: "/app/views/instance/index.html"
    });

    $routeProvider.when("/instance/users", {
        controller: "indexInstanceController",
        templateUrl: "/app/views/instance/users.html"
    });

    //---- plugins

    $routeProvider.when("/plugins/index", {
        controller: "indexPluginsController",
        templateUrl: "/app/views/plugins/index.html"
    });

    $routeProvider.when("/plugins/upload", {
        controller: "indexPluginsController",
        templateUrl: "/app/views/plugins/upload.html"
    });


    $routeProvider.otherwise({ redirectTo: "/home" });

}]);

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});

app.run(['authService', function (authService) {
    authService.fillAuthData();
}]);


