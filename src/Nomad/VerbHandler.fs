namespace Nomad.Verbs

open Nomad
open Nomad.Errors
open HttpHandler

/// A handler for each Http Verb
type VerbHandler<'T> =
    {
        /// A handler for GET requests
        Get : HttpHandler<'T>
        /// A handler for POST requests
        Post : HttpHandler<'T>
        /// A handler for PUT requests
        Put : HttpHandler<'T>
        /// A handler for PATCH requests
        Patch : HttpHandler<'T>
        /// A handler for DELETE requests
        Delete : HttpHandler<'T>
    }

module HttpHandler =
    /// A set of default http verb handlers, they return Error 405 : Method Not Allowed in response to all requests.  Use `defaultVerbs with` syntax to specify specific verb handlers.
    let defaultVerbs =
        {
            Get    = methodNotAllowed
            Post   = methodNotAllowed
            Put    = methodNotAllowed
            Patch  = methodNotAllowed
            Delete = methodNotAllowed
        }

    /// Handle a set of http verb handlers
    let handleVerbs (verbHandler : VerbHandler<'T>) = 
        HttpHandler (fun ctx -> 
            match Http.tryCreateRequestMethodFromString ctx.Request.Method with
            |Some Get    -> InternalHandlers.runHandler verbHandler.Get ctx
            |Some Post   -> InternalHandlers.runHandler verbHandler.Post ctx
            |Some Put    -> InternalHandlers.runHandler verbHandler.Put ctx
            |Some Patch  -> InternalHandlers.runHandler verbHandler.Patch ctx
            |Some Delete -> InternalHandlers.runHandler verbHandler.Delete ctx
            |None        -> InternalHandlers.runHandler (Errors.HttpHandler.methodNotAllowed *> HttpHandler.terminate) ctx
        )
